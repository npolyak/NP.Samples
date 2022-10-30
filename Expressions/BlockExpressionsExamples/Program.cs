using System.Linq.Expressions;
using System.Reflection;


public class Program
{
    // the block expression below will 
    // return the following code (i1 + i2)^2
    static void BlockSampleWithLocalVariableAndLineByLineCode()
    {
        var i1Expr = Expression.Parameter(typeof(int), "i1");
        var i2Expr = Expression.Parameter(typeof(int), "i2");

        // local variable 'result' expression
        var resultExpr = Expression.Parameter(typeof(int), "result");

        var blockExpr =
            Expression.Block
            (
                typeof(int), // return type of the block (skip parameter if void)
                new ParameterExpression[] { resultExpr }, // local variable(s)
                Expression.Assign(resultExpr, Expression.Add(i1Expr, i2Expr)), // result = i1 + i2; // line 1 of block expr
                Expression.MultiplyAssign(resultExpr, resultExpr),             // result *= result; // line 2
                resultExpr                                                     // return result;    // line 3
            );

        var lambdaExpr = Expression.Lambda<Func<int, int, int>>
        (
            blockExpr,  // lambda body expression 
            i1Expr,     // lambda input arg 1 
            i2Expr      // lambda input arg 2
        );

        var lambda = lambdaExpr.Compile();

        int i1 = 1, i2 = 2;
        var result = lambda(i1, i2); // (1 + 2)^2 = 9

        // should print (1 + 2) * (1 + 2) = 9
        Console.WriteLine($"({i1} + {i2}) * ({i1} + {i2}) = {result} ");
    }

    static void BlockLoopSumUpNumbersFromToSample()
    {
        // loop index expression
        var loopIdxExpr = Expression.Parameter(typeof(int), "i");

        // max index to iterate to
        var toExpr = Expression.Parameter(typeof(int), "to");

        // result 
        var resultExpr = Expression.Parameter(typeof(int), "result");

        // of type int returns the integer result
        var loopLabel = Expression.Label(typeof(int), "breakLabel");

        var blockExpr =
            Expression.Block
            (
                typeof(int), // returns int
                new ParameterExpression[] { resultExpr }, // resultExpr is the local variable expression
                Expression.Assign(resultExpr, Expression.Constant(0)), // result = 0; // initialize result
                Expression.Loop
                (
                    Expression.IfThenElse
                    (
                        Expression.LessThanOrEqual(loopIdxExpr, toExpr), // if (i < to) {
                        Expression.Block // block containing multiple lines of expressions
                                         // (it is more clearer when i++ is on a separate line)
                        (
                            Expression.AddAssign(resultExpr, loopIdxExpr),         // result += i;
                            Expression.PostIncrementAssign(loopIdxExpr)            // i++;
                        ),
                        Expression.Break(loopLabel, resultExpr)          // } else break the loop and return result
                    ),
                    loopLabel
                )
            );


        // unnecessary lambda parameter - resultExpr since we cannot define and instantiate a local variable without Block expression
        var sumNumbersFromTooLambdaExpr = Expression.Lambda<Func<int, int, int>>(blockExpr, loopIdxExpr, toExpr);

        var sumNumbersFromTooLambda = sumNumbersFromTooLambdaExpr.Compile();

        int from = 1;
        int to = 10;

        var sumResult = sumNumbersFromTooLambda(from, to);

        // prints 'Sum of integers from 1 to 10 is 55'
        Console.WriteLine($"Sum of intergers from {from} to {to} is {sumResult}");
    }

    // i1 is passed as a ref int and contains the result.
    public static void PlusRef(ref int i1, int i2)
    {
        i1 += i2;
    }

    /// <summary>
    /// we are wrapping a call to PlusRef method in a labmda
    /// The final expression code will look like:
    /// 
    /// (int i1, int i2) =&gt;
    /// {
    ///     int result = i1;
    ///     PlusRef(ref result, i2);
    /// 
    ///     return result;
    /// }
    /// 
    /// Since labmda cannot have 'ref' arguments, the lambda will take two integers and 
    /// will return an integer. 
    /// 
    /// The ref argument passed to PlusRef has to be defined as a local variable. Because of the 
    /// need to define a local variable we are forced to use a Block expression.
    /// </summary>
    static void BlockCallPlusRefSample()
    {
        // i1 argument expression
        var i1Expr = Expression.Parameter(typeof(int), "i1");

        // i2 argument expression
        var i2Expr = Expression.Parameter(typeof(int), "i2");

        // local variable 'result' expression
        var resultExpr = Expression.Parameter(typeof(int), "result");

        Type plusRefMethodContainer = typeof(Program);

        // PlusRef(...) MethodInfo
        MethodInfo plusRefMethodInfo =
            plusRefMethodContainer.GetMethod(nameof(PlusRef))!;

        // block expression
        var blockExpr = Expression.Block
        (
            typeof(int), // block return type
            new ParameterExpression[] { resultExpr },               // int result; // local variable
            Expression.Assign(resultExpr, i1Expr),                  // result = i1;
            Expression.Call(plusRefMethodInfo, resultExpr, i2Expr), // call PlusRef(ref result, i2)
            resultExpr                                              // return result;
        );

        var lambdaExpr =
            Expression.Lambda<Func<int, int, int>>
            (
                blockExpr, // lambda body expression
                i1Expr,    // i1 parameter expression
                i2Expr     // i2 parameter expression
            );

        var lambda = lambdaExpr.Compile();

        int i1 = 1, i2 = 2;
        int result = lambda(i1, i2);

        Console.WriteLine($"{i1} + {i2} = {result}");
        // prints 1 + 2 = 3 to console
    }

    public static void Main()
    {
        // BlockSampleWithLocalVariableAndLineByLineCode();

        // BlockLoopSumUpNumbersFromToSample();

        BlockCallPlusRefSample();
    }
}