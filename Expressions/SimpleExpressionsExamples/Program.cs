// See https://aka.ms/new-console-template for more information

using System.Linq.Expressions;
using System.Reflection;

// this file contains Simple (non-Block) expression samples

static void WrongConstantAssignmentSample()
{
    // Create variable 'myVar' of type 'int'
    var parameterExpression = Expression.Parameter(typeof(int), "myVar");

    // create constant 5
    var constExpression = Expression.Constant(5, typeof(int));

    // assing constant to the variable
    var assignExpression = Expression.Assign(parameterExpression, constExpression);

    // create lambda expression
    Expression<Action> lambdaExpr = 
        Expression.Lambda<Action>(assignExpression);
    ///.Lambda #Lambda1<System.Action>() {
    ///    $myVar = 5
    ///}

    // compile lambda expression (throws an exception since variable "myVar" is used, but not defined)
    Action lambda = lambdaExpr.Compile();

    // run lambda
    lambda();
}

static void CorrectedConstantAssignmentSample()
{
    // Create variable 'myVar' of type 'int'
    var paramExpression = Expression.Parameter(typeof(int), "myVar");

    // create constant 5
    var constExpression = Expression.Constant(5, typeof(int));

    // assing constant to the variable
    var assignExpression = Expression.Assign(paramExpression, constExpression);

    // create lambda expression (now it has an input parameter)
    Expression<Action<int>> lambdaExpr =
        Expression.Lambda<Action<int>>(assignExpression, paramExpression);
    ///.Lambda #Lambda1<System.Action`1[System.Int32]>(System.Int32 $myVar) {
    ///    $myVar = 5
    ///}

    // compile lambda expression
    Action<int> lambda = lambdaExpr.Compile();

    // run lambda (pass any int number to it)
    lambda(0);
}

static void ConstantAssignmentSampleWithPrintingResult()
{
    // Create variable 'myVar' of type 'int'
    var paramExpression = Expression.Parameter(typeof(int), "myVar");

    // create constant 5
    var constExpression = Expression.Constant(5, typeof(int));

    // assing constant to the variable
    var assignExpression = Expression.Assign(paramExpression, constExpression);

    // get method info for a WriteLine(int i) method
    MethodInfo writeLineMethodInfo = 
        typeof(Console).GetMethod(nameof(Console.WriteLine), new Type[] {typeof(int)})!;

    var callExpression = Expression.Call(writeLineMethodInfo, assignExpression);

    // create lambda expression (now it has an input parameter)
    Expression<Action<int>> lambdaExpr =
        Expression.Lambda<Action<int>>(callExpression, paramExpression);
    ///.Lambda #Lambda1<System.Action`1[System.Int32]>(System.Int32 $myVar) {
    ///    .Call System.Console.WriteLine($myVar = 5)
    ///}

    // compile lambda expression
    Action<int> lambda = lambdaExpr.Compile();

    // run lambda (pass any int number to it)
    lambda(0);
}

static void SimpleReturnConstantSample()
{
    var lambdaExpr = Expression.Lambda<Func<int>>(Expression.Constant(1234));
    ///.Lambda #Lambda1<System.Func`1[System.Int32]>() {
    ///    1234
    ///}

    var lambda = lambdaExpr.Compile();

    int returnedNumber = lambda();

    Console.WriteLine(returnedNumber);
}

static void ReturnSumSample()
{
    var i1Expr = Expression.Parameter(typeof(int), "i1");
    var i2Expr = Expression.Parameter(typeof(int), "i2");

    var sumExpr = Expression.Add(i1Expr, i2Expr);
    ///$i1 + $i2

    var sumLambdaExpr = Expression.Lambda<Func<int, int, int>>(sumExpr, i1Expr, i2Expr);
    ///.Lambda #Lambda1<System.Func`3[System.Int32,System.Int32,System.Int32]>(
    ///    System.Int32 $i1,
    ///    System.Int32 $i2)
    ///{
    ///    $i1 + $i2
    ///}

    var sumLambda = sumLambdaExpr.Compile();

    int i1 = 1, i2 = 2;
    int result = sumLambda(i1, i2);

    Console.WriteLine($"{i1} + {i2} = {result}");
}


static void LoopSample()
{
    var loopIdxExpr = Expression.Parameter(typeof(int), "i");

    var loopIdxToBreakOnExpr = Expression.Variable(typeof(int), "loopIdxToBreakOn");

    LabelTarget label = Expression.Label(typeof(int));

    var loopExpression =
        Expression.Loop
        (
            Expression.IfThenElse(
                Expression.LessThan(loopIdxExpr, loopIdxToBreakOnExpr),
                Expression.PostIncrementAssign(loopIdxExpr),
                Expression.Break(label, loopIdxExpr)
            ),
            label
        );
    ///.Loop
    ///{
    ///    .If($i < $loopIdxToBreakOn) {
    ///        $i++
    ///    } .Else {
    ///        .Break #Label1 { $i }
    ///    }
    ///}
    ///.LabelTarget #Label1:

    var lambdaExpr = Expression.Lambda<Func<int, int, int>>(loopExpression, loopIdxExpr, loopIdxToBreakOnExpr);
    ///.Lambda #Lambda1<System.Func`3[System.Int32,System.Int32,System.Int32]>(
    ///    System.Int32 $i,
    ///    System.Int32 $loopIdxToBreakOn) 
    ///{
    ///    .Loop  {
    ///        .If($i < $loopIdxToBreakOn) {
    ///            $i++
    ///        } .Else {
    ///            .Break #Label1 { $i }
    ///        }
    ///    }
    ///    .LabelTarget #Label1:
    ///}

    var lambda = lambdaExpr.Compile();

    int result = lambda(0, 5);

    Console.WriteLine(result);
}

static void LoopForCopyingArrayValuesSample()
{
    // assume that the passed arrays are of the same length
    var sourceArrayExpr = Expression.Parameter(typeof(int[]), "sourceArray");
    var targetArrayExpr = Expression.Parameter(typeof(int[]), "targetArray");
    var arrayCellIdxExpr = Expression.Parameter(typeof(int), "i");

    var arrayLengthExpr = Expression.ArrayLength(sourceArrayExpr); // sourceArray.Length

    var loopLabel = Expression.Label();

    var loopExpr =
        Expression.Loop
        (
            Expression.IfThenElse
            (
                Expression.LessThan(arrayCellIdxExpr, arrayLengthExpr),                                         // if (i < sourceArray.Length)
                Expression.Assign(
                    Expression.ArrayAccess(targetArrayExpr, arrayCellIdxExpr),                                  //     targetArray[i] = 
                    Expression.ArrayAccess(sourceArrayExpr, Expression.PostIncrementAssign(arrayCellIdxExpr))   //         sourceArray[i++];
                ),
                Expression.Break(loopLabel)
            ),
            loopLabel
        );
    ///.Loop  
    ///{
    ///    .If($i < $sourceArray.Length) {
    ///        $targetArray[$i] = $sourceArray[$i++]
    ///    } .Else {
    ///        .Break #Label1 { }
    ///    }
    ///}
    ///.LabelTarget #Label1:


    // unnecessary lambda parameter - arrayCellIdxExpr since we cannot define and instantiate a local variable without Block expression
    var arrayCopyLambdaExpr = 
        Expression.Lambda<Action<int[], int[], int>>(loopExpr, sourceArrayExpr, targetArrayExpr, arrayCellIdxExpr);
    ///.Lambda #Lambda1<System.Action`3[System.Int32[],System.Int32[],System.Int32]>(
    ///    System.Int32[] $sourceArray,
    ///    System.Int32[] $targetArray,
    ///    System.Int32 $i) 
    ///{
    ///    .Loop  
    ///    {
    ///        .If($i < $sourceArray.Length) {
    ///            $targetArray[$i] = $sourceArray[$i++]
    ///        } .Else {
    ///            .Break #Label1 { }
    ///        }
    ///    }
    ///    .LabelTarget #Label1:
    ///}

var arrayCopyLambda = arrayCopyLambdaExpr.Compile();

    int[] sourceArray = Enumerable.Range(1, 10).ToArray();

    int[] targetArray = new int[10];

    arrayCopyLambda(sourceArray, targetArray, 0);

    Console.WriteLine(string.Join(", ", targetArray));
    // will print: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
}

static void LoopSumUpNumbersFromToSample()
{
    var loopIdxExpr = Expression.Parameter(typeof(int), "i");
    var toExpr = Expression.Parameter(typeof(int), "to");
    var resultExpr = Expression.Parameter(typeof(int), "result");

    var loopLabel = Expression.Label(typeof(int));

    var loopExpr =
        Expression.Loop
        (
            Expression.IfThenElse
            (
                Expression.LessThanOrEqual(loopIdxExpr, toExpr), // if (i < to)
                Expression.AddAssign(resultExpr, Expression.PostIncrementAssign(loopIdxExpr)),   //      result += i++;
                Expression.Break(loopLabel, resultExpr)          // else break the loop and return result
            ),
            loopLabel
        );
    ///.Loop  
    ///{
    ///    .If($i <= $to) {
    ///        $result += $i++
    ///    } .Else {
    ///        .Break #Label1 { $result }
    ///    }
    ///}
    ///.LabelTarget #Label1:

    // unnecessary lambda parameter - resultExpr since we cannot define and instantiate a local variable without Block expression
    var sumNumbersFromTooLambdaExpr = Expression.Lambda<Func<int, int, int, int>>(loopExpr, loopIdxExpr, toExpr, resultExpr);
    ///.Lambda #Lambda1<System.Func`4[System.Int32,System.Int32,System.Int32,System.Int32]>(
    ///    System.Int32 $i,
    ///    System.Int32 $to,
    ///    System.Int32 $result) 
    ///{
    ///    .Loop  {
    ///        .If($i <= $to) {
    ///            $result += $i++
    ///        } .Else {
    ///    }
    ///    .LabelTarget #Label1:
    ///}

    var sumNumbersFromTooLambda = sumNumbersFromTooLambdaExpr.Compile();

    int from = 1; 
    int to = 10;

    var sumResult = sumNumbersFromTooLambda(from, to, 0);

    Console.WriteLine($"Sum of intergers from {from} to {to} is {sumResult}");
}


//WrongConstantAssignmentSample();

//CorrectedConstantAssignmentSample();

//ConstantAssignmentSampleWithPrintingResult();

//SimpleReturnConstantSample();

//ReturnSumSample();

//LoopSample();

//LoopForCopyingArrayValuesSample();

//LoopSumUpNumbersFromToSample();