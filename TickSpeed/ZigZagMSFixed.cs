using System.Collections.Generic;
using TSLab.Script.Handlers;


namespace TickSpeed
{
    // ZigZag
    [HandlerCategory("Arelyt")]
    [HandlerName("ZigZagMSFx")]
    public class ZigZagMSFixedClass : IDouble2DoubleHandler
	{
		[HandlerParameter(true, "0.3", Min = "0", Max = "10", Step = "0.1")]
		public double ExtProcent { get; set; }

		[HandlerParameter(true, "0", Min = "0", Max = "1", Step = "1")]
		public int ExtCurrentBar { get; set; }
		
		[HandlerParameter(true, "0", Min = "0", Max = "1", Step = "1")]
		public int Direction { get; set; }
		
		[HandlerParameter(true, "0", Min = "0", Max = "1", Step = "1")]
		public int Interpolation { get; set; }

		public IList<double> Execute(IList<double> source)
		{
			var P = source;
			var ZZMS = new double[P.Count];
			var OutValue = new double[P.Count];
		    int i0=0;
			
			var Procent = ExtProcent*0.01; 
			
			for(var i = 0; i < P.Count; i++)
			{
				ZZMS[i]=0;
			}
			
			var min = P[1];
			var max = min;
			var m = 0;
			
			// Расчет индикатора Zig Zag
			for(var i = 2; i < P.Count; i++)
			{
				if (P[i]>max)
  				{
					max=P[i];
					if (m!=2)//m=1:building the down-point (min) of ZigZag
   					{
						if (max-min>=Procent*min)//min (m!=2) end,max (m=2) begin
    					{
							m=2;
							ZZMS[i]=max;
//							i0=i;
							min=max;
    					}
					 	else ZZMS[i]=0.0;//max-min=miser,(m!=2) continue
   					}
					else //max (m=2) continue
   					{
//						ZZMS[i0]=0.0;
						ZZMS[i]=max;
//						i0=i;
						min=max;
  					}
				}
				else 
					if (P[i]<min)
  					{
						min=P[i];
   						if (m!=1)//m=2:building the up-point (max) of ZigZag
   						{
   							if (max-min>=Procent*max)//max (m!=1) end,min (m=1) begin
    						{
   								m=1;
   								ZZMS[i]=min;
//   								i0=i;
   								max=min;
    						}
   							else ZZMS[i]=0.0;//max-min=miser,(m!=1) continue
   						}
   						else //min (m=1) continue
   						{
//   							ZZMS[i0]=0.0;
   							ZZMS[i]=min;
//   							i0=i;
   							max=min;
  						}
					}
					else ZZMS[i]=0.0;
			}
			if (ZZMS[0]==0.0) ZZMS[0]=P[0];
			
			if(ExtCurrentBar==1)
			{	
				if(ZZMS[P.Count-1]==0) ZZMS[P.Count-1]=P[P.Count-1];
			}
			OutValue = ZZMS;
			// Заверщение расчета индикатора Zig Zag
			// Расчет направления Direction
			if(Direction==1 && Interpolation==0)
			{
        		var Peaks = new double[P.Count];
        		double CurPeak=0, PrevPeak=0;
            
        		for(int i = 0; i < P.Count; i++)
            	{
        			Peaks[i]=0;
            	}
        	
	            for(int i = 0; i < P.Count; i++)
	            {
	            	if(ZZMS[i]!=0)
	            	{
	            		CurPeak=ZZMS[i];
	            		for(int j = i-1 ; j >= 0; j--)
	            		{
	            			if(ZZMS[j]!=0)
	            			{
	            				PrevPeak = ZZMS[j];
	            				break;
	            			}
	            			else PrevPeak = 0;
	            		}
	            		if(PrevPeak!=0)
	            		{
	            			if (CurPeak>PrevPeak) Peaks[i]=1;
	            			else Peaks[i]=-1;	
	            		}
	            	}
	            }	
	        	OutValue = Peaks;	
			}
			// Заверщение расчета Direction
			// Расчет направления Intepolation
			if(Direction==0 && Interpolation==1)
			{
	        	var ZZI = new double[P.Count];
	        	double Fi=0, Fj=0, K=0, b=0;
	        	int xi=0, xj=0;
	            // уравнение прямой y = K*x+b
	            
	        	for(int i = 0; i < P.Count; i++)
	            {
	        		ZZI[i]=ZZMS[i];
	            }
	        	
	            for(int i = 0; i < P.Count; i++)
	            {
	            	if(ZZMS[i]!=0)
	            	{
	            		Fi=ZZMS[i];
	            		xi=i;
	            		for(int j = i-1 ; j >= 0; j--)
	            		{
	            			if(ZZMS[j]!=0)
	            			{
	            				Fj = ZZMS[j];
	            				xj=j;
	            				break;
	            			}
	            			else 
	            			{
	            				Fj = ZZMS[0];
	            				xj=0;
	            			}
	            		}
	            		if(Fj!=0)
	            		{
	            			K = (Fi-Fj)/(xi-xj);
	            			b = Fi - K * xi;
	            		}
	            		for(int l = xj+1; l < xi; l++) ZZI[l] = K*l+b;
	            	}
	            }
	            OutValue = ZZI;
			}
			// Заверщение расчета Intepolation
			if(Direction==1 && Interpolation==1)
			{
				OutValue = ZZMS;
			}
			return OutValue;
		}
	}
}