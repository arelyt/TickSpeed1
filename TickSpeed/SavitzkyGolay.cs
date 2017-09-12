// Decompiled with JetBrains decompiler
// Type: Altaxo.Calc.Regression.SavitzkyGolay
// Assembly: AltaxoCore, Version=4.6.1561.1, Culture=neutral, PublicKeyToken=null
// MVID: 65D0C866-0B9D-4CD2-BB54-5CF39231937A
// Assembly location: C:\Program Files (x86)\Altaxo\bin\AltaxoCore.dll

using Altaxo.Calc.LinearAlgebra;
using System;
using System.Collections.Generic;
using Altaxo.Calc.Regression;

namespace TickSpeed
{
  /// <summary>
  /// SavitzkyGolay implements the calculation of the Savitzky-Golay filter coefficients and their application
  /// to smoth data, and to calculate derivatives.
  /// </summary>
  /// <remarks>Ref.: "Numerical recipes in C", chapter 14.8</remarks>
  public class SavitzkyGolay
  {
    /// <summary>Filters to apply to the left edge of the array.</summary>
    private double[][] _left;
    /// <summary>Filters to apply to the right edge of the array. Note: the rightmost filter is in index 0</summary>
    private double[][] _right;
    /// <summary>Filter to apply to the middle of the array.</summary>
    private double[] _middle;

    /// <summary>Calculate Savitzky-Golay coefficients.</summary>
    /// <param name="leftpoints">Points on the left side included in the regression.</param>
    /// <param name="rightpoints">Points to the right side included in the regression.</param>
    /// <param name="derivativeorder">Order of derivative for which the coefficients are calculated.</param>
    /// <param name="polynomialorder">Order of the regression polynomial.</param>
    /// <param name="coefficients">Output: On return, contains the calculated coefficients.</param>
    public static void GetCoefficients(int leftpoints, int rightpoints, int derivativeorder, int polynomialorder, IVector<double> coefficients)
    {
      int length = leftpoints + rightpoints + 1;
      if (leftpoints < 0)
        throw new ArgumentException("Argument leftpoints must not be <=0!");
      if (rightpoints < 0)
        throw new ArgumentException("Argument rightpoints must not be <=0!");
      if (length <= 1)
        throw new ArgumentException("Argument leftpoints and rightpoints must not both be zero!");
      if (polynomialorder >= length)
        throw new ArgumentException("Argument polynomialorder must not be smaller than total number of points");
      if (derivativeorder > polynomialorder)
        throw new ArgumentException("Argument derivativeorder must not be greater than polynomialorder!");
      if (coefficients == null || coefficients.Length < length)
        throw new ArgumentException("Vector of coefficients is either null or too short");
      Matrix matrix1 = new Matrix(polynomialorder + 1, polynomialorder + 1);
      double[] vector = new double[length];
      for (int index = 0; index < length; ++index)
        vector[index] = 1.0;
      for (int index1 = 0; index1 <= polynomialorder; ++index1)
      {
        double num = vector.Sum();
        for (int index2 = 0; index2 <= index1; ++index2)
          matrix1[index1 - index2, index2] = num;
        for (int index2 = 0; index2 < length; ++index2)
          vector[index2] *= (double) (index2 - leftpoints);
      }
      for (int index1 = polynomialorder - 1; index1 >= 0; --index1)
      {
        double num = vector.Sum();
        for (int index2 = 0; index2 <= index1; ++index2)
          matrix1[polynomialorder - index2, polynomialorder - index1 + index2] = num;
        for (int index2 = 0; index2 < length; ++index2)
          vector[index2] *= (double) (index2 - leftpoints);
      }
      ILuDecomposition luDecomposition = matrix1.GetLuDecomposition();
      Matrix matrix2 = new Matrix(polynomialorder + 1, 1);
      matrix2[derivativeorder, 0] = 1.0;
      Matrix matrix3 = matrix2;
      IMapackMatrix mapackMatrix = luDecomposition.Solve((IMapackMatrix) matrix3);
      for (int index1 = -leftpoints; index1 <= rightpoints; ++index1)
      {
        double num1 = 0.0;
        double num2 = 1.0;
        int index2 = 0;
        while (index2 <= polynomialorder)
        {
          num1 += ((IMatrix<double>) mapackMatrix)[index2, 0] * num2;
          ++index2;
          num2 *= (double) index1;
        }
        coefficients[index1 + leftpoints] = num1;
      }
    }

    /// <summary>This sets up a Savitzky-Golay filter.</summary>
    /// <param name="numberOfPoints">Number of points. Must be an odd number, otherwise it is rounded up.</param>
    /// <param name="derivativeOrder">Order of derivative you want to obtain. Set 0 for smothing.</param>
    /// <param name="polynomialOrder">Order of the fitting polynomial. Usual values are 2 or 4.</param>
    public SavitzkyGolay(int numberOfPoints, int derivativeOrder, int polynomialOrder)
    {
      numberOfPoints = 1 + 2 * (numberOfPoints / 2);
      int num = (numberOfPoints - 1) / 2;
      this._left = JaggedArrayMath.GetMatrixArray(num, numberOfPoints);
      this._right = JaggedArrayMath.GetMatrixArray(num, numberOfPoints);
      this._middle = new double[numberOfPoints];
      SavitzkyGolay.GetCoefficients(num, num, derivativeOrder, polynomialOrder, this._middle.ToVector());
      for (int index = 0; index < num; ++index)
      {
        SavitzkyGolay.GetCoefficients(index, 2 * num - index, derivativeOrder, polynomialOrder, this._left[index].ToVector());
        SavitzkyGolay.GetCoefficients(2 * num - index, index, derivativeOrder, polynomialOrder, this._right[index].ToVector());
      }
    }

    /// <summary>This sets up a Savitzky-Golay filter.</summary>
    /// <param name="parameters">Set of parameters used for Savitzky-Golay filtering.</param>
    public SavitzkyGolay(SavitzkyGolayParameters parameters)
      : this(parameters.NumberOfPoints, parameters.DerivativeOrder, parameters.PolynomialOrder)
    {
    }

    /// <summary>
    /// This applies the set-up filter to an array of numbers. The left and right side is special treated by
    /// applying Savitzky-Golay with appropriate adjusted left and right number of points.
    /// </summary>
    /// <param name="array">The array of numbers to filter.</param>
    /// <param name="result">The resulting array. Must not be identical to the input array!</param>
    public void Apply(double[] array, double[] result)
    {
      int length = this._middle.Length;
      int num1 = (length - 1) / 2;
      if (array == result)
        throw new ArgumentException("Argument array and result must not be identical!");
      if (array.Length < length)
        throw new ArgumentException("Input array must have same or greater length than the filter!");
      for (int index1 = 0; index1 < num1; ++index1)
      {
        double[] numArray = this._left[index1];
        double num2 = 0.0;
        for (int index2 = 0; index2 < length; ++index2)
          num2 += array[index2] * numArray[index2];
        result[index1] = num2;
      }
      int num3 = array.Length - length;
      for (int index1 = 0; index1 <= num3; ++index1)
      {
        double num2 = 0.0;
        for (int index2 = 0; index2 < length; ++index2)
          num2 += array[index1 + index2] * this._middle[index2];
        result[index1 + num1] = num2;
      }
      int num4 = array.Length - length;
      int num5 = array.Length - 1;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        double[] numArray = this._right[index1];
        double num2 = 0.0;
        for (int index2 = 0; index2 < length; ++index2)
          num2 += array[num4 + index2] * numArray[index2];
        result[num5 - index1] = num2;
      }
    }

    /// <summary>
    /// This applies the set-up filter to an array of numbers. The left and right side is special treated by
    /// applying Savitzky-Golay with appropriate adjusted left and right number of points.
    /// </summary>
    /// <param name="array">The array of numbers to filter.</param>
    /// <param name="result">The resulting array. Must not be identical to the input array!</param>
    public void Apply(IReadOnlyList<double> array, IVector<double> result)
    {
      int length = this._middle.Length;
      int num1 = (length - 1) / 2;
      if (array == result)
        throw new ArgumentException("Argument array and result must not be identical!");
      if (array.Count < length)
        throw new ArgumentException("Input array must have same or greater length than the filter!");
      for (int index1 = 0; index1 < num1; ++index1)
      {
        double[] numArray = this._left[index1];
        double num2 = 0.0;
        for (int index2 = 0; index2 < length; ++index2)
          num2 += array[index2] * numArray[index2];
        result[index1] = num2;
      }
      int num3 = array.Count - length;
      for (int index1 = 0; index1 <= num3; ++index1)
      {
        double num2 = 0.0;
        for (int index2 = 0; index2 < length; ++index2)
          num2 += array[index1 + index2] * this._middle[index2];
        result[index1 + num1] = num2;
      }
      int num4 = array.Count - length;
      int num5 = array.Count - 1;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        double[] numArray = this._right[index1];
        double num2 = 0.0;
        for (int index2 = 0; index2 < length; ++index2)
          num2 += array[num4 + index2] * numArray[index2];
        result[num5 - index1] = num2;
      }
    }
  }
}
