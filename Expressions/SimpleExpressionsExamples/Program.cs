// See https://aka.ms/new-console-template for more information

using System.Linq.Expressions;
using System.Reflection;

// this file contains Simple (non-Block) expression samples

static void WrongConstantAssignmentSample()
{
    // Create variable 'myVar' of type 'int'
    var paramExpression = Expression.Parameter(typeof(int), "myVar");

    // create constant 5
    var constExpression = Expression.Constant(5, typeof(int));

    // assign constant to the variable
    var assignExpression = 
        Expression.Assign
        (
            paramExpression, // lambda body expression
            constExpression  // lambda input parameter expression
        );

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

    // assign constant to the variable
    var assignExpression = Expression.Assign(paramExpression, constExpression);

    // create lambda expression (now it has an input parameter)
    Expression<Action<int>> lambdaExpr =
        Expression.Lambda<Action<int>>
        (
            assignExpression, // lambda body expression
            paramExpression   // lambda input parameter expression
        );
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

    // get method info for a Console.WriteLine(int i) method
    MethodInfo writeLineMethodInfo = 
        typeof(Console).GetMethod(nameof(Console.WriteLine), new Type[] {typeof(int)})!;

    // we create an expression to call Console.WriteLine(int i)
    var callExpression = Expression.Call(writeLineMethodInfo, assignExpression);

    // create lambda expression (now it has an input parameter)
    Expression<Action<int>> lambdaExpr =
        Expression.Lambda<Action<int>>
    (
            callExpression, /* lambda body expression */
            paramExpression /* input parameter expression */
    );
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
    // create a lambda expression returning integer 1234
    var lambdaExpr = Expression.Lambda<Func<int>>(Expression.Constant(1234, typeof(int)));
    ///.Lambda #Lambda1<System.Func`1[System.Int32]>() {
    ///    1234
    ///}

    // compile lambda expression
    var lambda = lambdaExpr.Compile();

    // lambda returns 1234
    int returnedNumber = lambda();

    // 1234 is printed to console
    Console.WriteLine(returnedNumber);
}

static void ReturnSumSample()
{
    // integer input parameter i1
    var i1Expr = Expression.Parameter(typeof(int), "i1");

    // integer input parameter i2
    var i2Expr = Expression.Parameter(typeof(int), "i2");

    // sum up two numbers expression
    var sumExpr = Expression.Add(i1Expr, i2Expr);
    ///$i1 + $i2

    // lambda expression that sums up two  numbers and returns the result
    var sumLambdaExpr = 
        Expression.Lambda<Func<int, int, int>>
        (
            sumExpr, // lambda body expression
            i1Expr,  // first int parameter i1 expression
            i2Expr   // second int parameter i2 expression
       );
    ///.Lambda #Lambda1<System.Func`3[System.Int32,System.Int32,System.Int32]>(
    ///    System.Int32 $i1,
    ///    System.Int32 $i2)
    ///{
    ///    $i1 + $i2
    ///}

    // compile lambda expression
    var sumLambda = sumLambdaExpr.Compile();

    int i1 = 1, i2 = 2;

    // run lambda (i1 + i2)
    int result = sumLambda(i1, i2);

    // print the result
    Console.WriteLine($"{i1} + {i2} = {result}");
}


static void LoopSample()
{
    // loop index
    var loopIdxExpr = Expression.Parameter(typeof(int), "i");

    var loopIdxToBreakOnExpr = Expression.Parameter(typeof(int), "loopIdxToBreakOn");

    // label with return type int will be returned when loop breaks. 
    LabelTarget breakLabel = Expression.Label(typeof(int), "breakLoop");

    // loop expression 
    var loopExpression =
        Expression.Loop 
        (
            // if then else expression
            Expression.IfThenElse(
                Expression.LessThan(loopIdxExpr, loopIdxToBreakOnExpr), // if (i < loopIdxToBreakOn)
                Expression.PostIncrementAssign(loopIdxExpr),            //     i++;
                Expression.Break(breakLabel, loopIdxExpr)               // else return i;
            ),
            breakLabel
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

    var lambdaExpr = Expression.Lambda<Func<int, int, int>>
    (
        loopExpression,      // loop lambda expression body
        loopIdxExpr,         // loop index (we cannot define it as a local variable, so, instead we pass it as an input arg)
        loopIdxToBreakOnExpr // input arg expression specifying the number to break on when loop index reaches it.
    ); 
    ///.Lambda #Lambda1<System.Func`3[System.Int32,System.Int32,System.Int32]>(
    ///    System.Int32 $i,
    ///    System.Int32 $loopIdxToBreakOn) 
    ///{
    ///    .Loop  {
    ///        .If($i < $loopIdxToBreakOn) {
    ///            $i++
    ///        } .Else {
    ///            .Break #breakLoop { $i }
    ///        }
    ///    }
    ///    .LabelTarget #breakLoop:
    ///}

    var lambda = lambdaExpr.Compile();

    int result = lambda(0, 5);

    // should print 5
    Console.WriteLine(result);
}

static void LoopForCopyingArrayValuesSample()
{
    // assume that the passed arrays are of the same length

    // source array expression
    var sourceArrayExpr = Expression.Parameter(typeof(int[]), "sourceArray");

    // target array expression
    var targetArrayExpr = Expression.Parameter(typeof(int[]), "targetArray");

    // array cell index (we have to pass it as an input arg since there are no local variables)
    var arrayCellIdxExpr = Expression.Parameter(typeof(int), "i");

    // source array length
    var arrayLengthExpr = Expression.ArrayLength(sourceArrayExpr); // sourceArray.Length

    // we do not specify the label type, so loopLabel is void
    var loopLabel = Expression.Label("breakLabel");

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
                Expression.Break(loopLabel)                                                                     // else break;
            ),
            loopLabel
        );
    ///.Loop  
    ///{
    ///    .If($i < $sourceArray.Length) {
    ///        $targetArray[$i] = $sourceArray[$i++]
    ///    } .Else {
    ///        .Break #breakLabel { }
    ///    }
    ///}
    ///.LabelTarget #breakLabel:


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
    ///            .Break #breakLabel { }
    ///        }
    ///    }
    ///    .LabelTarget #breakLabel:
    ///}

    var arrayCopyLambda = arrayCopyLambdaExpr.Compile();

    int[] sourceArray = Enumerable.Range(1, 10).ToArray();

    int[] targetArray = new int[10];

    arrayCopyLambda(sourceArray, targetArray, 0);

    // will print: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    Console.WriteLine(string.Join(", ", targetArray));
}

static void LoopSumUpNumbersFromToSample()
{
    // loop index expression
    var loopIdxExpr = Expression.Parameter(typeof(int), "i");

    // max index to iterate to
    var toExpr = Expression.Parameter(typeof(int), "to");

    // result 
    var resultExpr = Expression.Parameter(typeof(int), "result");

    // of type int returns the integer result
    var loopLabel = Expression.Label(typeof(int), "breakLabel");

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
    ///        .Break #breakLabel { $result }
    ///    }
    ///}
    ///.LabelTarget #breakLabel:

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
    ///        } 
    ///        .Else {
    ///             .Break #breakLabel { $result }
    ///        }
    ///    }
    ///    .LabelTarget #breakLabel:
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