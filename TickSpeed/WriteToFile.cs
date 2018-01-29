using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RusAlgo.Helper;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    /// <summary>
    /// Кубик сохраняет все что на него подали в текстовый файл. 
    /// Формат файла похож на финамовский. Имеется заголовок TICKER,PER,DATE,TIME,OPEN,HIGH,LOW,CLOSE,VOL и данные.
    /// Минимально можно сохранять инструмент и чтото еще. То есть два входа!
    /// Чтобы все работало корректно, соединяем выход кубика с графиком, иначе кодогенератор может выбросить кубик из расчета как ненужный
    /// 
    /// Тестовый скрипт: WriteToFile Handler Test.tscript
    /// </summary>
    [HandlerCategory("TickSpeed")]
#pragma warning disable 612
    [HandlerName("WriteToFile")]
#pragma warning restore 612
    [InputsCount(2, 200)]
#pragma warning disable 612
    [InputInfo(0, "Инструмент")]
#pragma warning restore 612
    public class WriteToFile : ITwoSourcesHandler, ISecurityInput0, IDoubleInputs, IDoubleReturns, IStreamHandler, IValuesHandlerWithNumber, IContextUses
    {
        private string[] _buffer;
        // ReSharper disable once InconsistentNaming
        private const string DEFAULT_HEADER = "<TICKER>|<PER>|<DATE>|<TIME>|<OPEN>|<HIGH>|<LOW>|<CLOSE>|<VOL>";
        
        public IContext Context { set; private get; }

        [HandlerParameter(Name = "Папка", Default = "%temp%", NotOptimized = true)]
        public string DirPath { get; set; }

        [HandlerParameter(Name = "Шапка", Default = "DATA0", NotOptimized = true)]
        public string Header { get; set; }

        /// <summary>
        /// Формат вывода вещественных чисел. Прям можно писать форматы C#
        /// </summary>
        [HandlerParameter(Name = "Формат", Default = "0.000000", NotOptimized = true)]
        public string RealFormat { get; set; }

        [HandlerParameter(Name = "Разделитель", Default = ",", NotOptimized = true)]
        public string Delimeter { get; set; }


        public IList<double> Execute(ISecurity sec, params IList<double>[] dataArr)
        {
            // пишем в файл
            using (var sw = MakeWriter(sec, false))
            {
                // заголовок
                var header = MakeHeader(dataArr.Length);
                sw.WriteLine(header);

                // число свечек, которые содержат полезные данные
                var count = Context.BarsCount;
                if (!Context.IsLastBarClosed)
                    count--;

                // данные
                var dataCount = dataArr.Length + 1;     // +1 - место для номера свечки оставляем
                var data = new double[dataCount];
                for (var bar = 0; bar < count; bar++)
                {
                    data[dataCount - 1] = bar;          // в последний элемент пихаем номер бара

                    for (var j = 0; j < dataArr.Length; j++)
                    {
                        var item = dataArr[j];
                        data[j] = item[bar];
                    }

                    var line = MakeLine(sec, data);

                    // пишем в файл строку для свечи bar
                    sw.WriteLine(line);
                }

                sw.Flush();
            }

            return new double[0];
        }

        public double Execute(ISecurity sec, params double[] dataArr)
        {
            // последний элемент массива это номер бара для которого вызвано все.
            var barNum = Convert.ToInt32(dataArr.Last());

            // число свечек, которые содержат полезные данные
            var count = Context.BarsCount;
            if (!Context.IsLastBarClosed)
                count--;

            // пишем данные в буфер, учитывая что может прилететь динамическая свечка, которую не надо учитывать
            _buffer = _buffer ?? new string[count];
            if (barNum < count)
            {
                _buffer[barNum] = MakeLine(sec, dataArr);
            }
            
            
            // пока не попали на последнюю полезную свечку, не пишем в файл
            if (barNum != count - 1) 
                return 0;

            // когда к нам попали данные для последней полезной свечки, мы тут же сейвим все в файл из буфера
            using (var sw = MakeWriter(sec, false))
            {
                // заголовок
                var header = MakeHeader(dataArr.Length - 1);
                sw.WriteLine(header);

                // данные из буфера
                foreach (var line in _buffer)
                    sw.WriteLine(line);

                sw.Flush();
            }

            return 0;
        }



        private StreamWriter MakeWriter(ISecurity sec, bool append)
        {
            // инициализируем файловый поток
            var per = sec.IntervalInstance.ToString();
            var ticker = sec.Symbol;
            var fileName = "{0}_{1}.txt".Put(ticker, per);
            var expandedDirPath = Environment.ExpandEnvironmentVariables(DirPath);
            var fullPath = Path.Combine(expandedDirPath, fileName);

            return new StreamWriter(fullPath, append, new UnicodeEncoding());
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private string MakeHeader(int dataCount)
        {
            // разбираем шапку введенную юзером для его данных
            var strItems = Header.Split(new[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries);

            if (strItems.Length != dataCount)
                throw new ArgumentException("Число элементов в заголовке не совпадает с числом данных.");

            // собираем общую шапку
            // сначала собираем заголовок используя |, потом подменяем их на нужный символ
            var headerStr = DEFAULT_HEADER;
            foreach (var strItem in strItems)
            {
                var s = strItem.Trim();
                headerStr += "|<{0}>".Put(s.ToUpper(), Delimeter);
            }

            var resultHeader = PlaceDelimeter(headerStr);
            return resultHeader;
        }

        private string MakeLine(ISecurity sec, params double[] dataArr)
        {
            // последний элемент массива это номер бара для которого вызвано все.
            var barNum = Convert.ToInt32(dataArr.Last());

            // формируем строку для инструмента используя разделитель по умолчанию ",". Потом подменяем ее на нужный Delimeter
            var per = sec.IntervalInstance.ToString();
            var ticker = sec.Symbol;
            var date = "{0:yyyyMMdd}".Put(sec.Bars[barNum].Date);

            var placeHolder = "{0:" + RealFormat + "}";
            var time = "{0:HHmmss}".Put(sec.Bars[barNum].Date);
            var open = placeHolder.Put(sec.Bars[barNum].Open);
            var high = placeHolder.Put(sec.Bars[barNum].High);
            var low = placeHolder.Put(sec.Bars[barNum].Low);
            var close = placeHolder.Put(sec.Bars[barNum].Close);
            var vol = placeHolder.Put(sec.Bars[barNum].Volume);
            
            var line = "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}".Put(ticker, per, date, time, open, high, low, close, vol);

            // добиваем ее данными индикаторов, не учитываем последний бар
            for (var i = 0; i < dataArr.Length - 1; i++ )
            {
                line += "|" + placeHolder.Put(dataArr[i]);
            }

            var resultLine = PlaceDelimeter(line);
            return resultLine;
        }

        private string PlaceDelimeter(string sourceStr)
        {
            return sourceStr.Replace("|", Delimeter);
        }
    }
}
