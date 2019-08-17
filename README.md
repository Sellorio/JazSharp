<style>
	.to-top {
		float: right;
	}
</style>

# JazSharp

## Contents

- [Introduction](#intro)
- [User Guide](#userGuide)
  - [Installation](#installation)
  - [Creating tests](#creatingTests)
    - [Creating the test class](#testClass)
    - [Describes (test scopes)](#describes)
    - [Its (tests)](#its)
    - [BeforeEach and AfterEach](#beforeAfterEach)
    - [Excluding and focusing](#excludingAndFocusing)
  - [Spying](#spying)
    - [What are spies?](#whatAreSpies)
    - [Behaviours](#behaviours)
    - [Sequences and Quantifiers](#sequences)
    - [Properties](#propertySpies)
    - [Create Spy](#createSpy)
    - [Calls](#calls)
    - [Limitations](#spyLimits)
  - [Expectations (asserts)](#expects)
    - [Spy Expectations](#spyExpects)
    - [Call Expectations](#callExpects)
    - [Value Expectations](#valueExpects)
    - [Matchers](#matchers)
- [Developer Guide](#developerGuide)
  - [Build and run](#buildAndRun)
  - [Map](#devMap)

<a id="intro"></a>
## Introduction

JazSharp is heavily inspired by Jasmine - the JavaScript unit testing framework. Angular developers
will be very familiar with it since that is the unit testing framework used in a CLI created default
app.

### Why was this made?

Originally, JazSharp was only aiming to be a mocking framework but after the initial implementation
attempts proved to be unreliable, the scope increased to be a full unit testing framework.

The benefits of JazSharp over other mocking frameworks are:

- No reliance on interfaces or similar layers of separation that are not motivated by clean development
practises.
- No issues when testing code that changes static/global values for the app - statics methods and
properties can be mocked just like any method.
- An alternative approach to defining mocks. This is less of a benefit and more of a personal preference.
For example, parameters can be checked after a mock is called and defining a mock is quick and easy.

The syntax used to define test methods is also very different from other .Net testing frameworks and allows
test names (or descriptions in the case of JazSharp) to read more clearly both to technical and non-technical
persons.

<a id="userGuide"></a>
## User Guide

<a id="installation"></a>
### Installation

To start using JazSharp, all you have to do is install the `JazSharp` and `JazSharp.TestAdapter` nuget packages
from nuget.org. The former is the core of the framework and the latter enables the framework to work with
Visual Studio\'s Test Explorer as well as allowing tests to be executed by calling `dotnet test`.

<a id="creatingTests"></a><a href="#jazsharp" class="to-top">To top</a>
### Creating tests

<a id="testClass"></a>
#### Creating the test class

A test class in JazSharp is any class that inherits from `JazSharp.Spec`. The class does not need to be public.
Unlike the common practise in JavaScript (when using Jasmine), it is recommended to place test classes in a
dedicated unit test assembly (as is popular when unit testing in .Net).

An empty test class would look something like this:

```C#
class FooSpec : Spec
{
    public FooSpec()
    {
    }
}
```

<a id="describes"></a><a href="#jazsharp" class="to-top">To top</a>
#### Describes (test scopes)

All tests need to be grouped into one or more levels of Describes. The first Describe should almost always be
used to state which class is being tested.

```C#
class FooSpec : Spec
{
    public FooSpec()
    {
        Describe<Foo>(() =>
        {
        });
    }
}
```

Note: for static types, you will need to revert to specifying the name manually.

```C#
class StaticFooSpec : Spec
{
    public StaticFooSpec()
    {
        Describe(nameof(StaticFoo), () =>
        {
        });
    }
}
```

The next Describe will usually be used to specify the method being tested.

```C#
class FooSpec : Spec
{
    public FooSpec()
    {
        Describe<Foo>(() =>
        {
            Describe(nameof(Foo.Bar), () =>
            {
            });
        });
    }
}
```

At this point it is common to start specifying your tests but sometimes additional
describes are used to group tests together by a particular scenario.

```C#
class FooSpec : Spec
{
    public FooSpec()
    {
        Describe<Foo>(() =>
        {
            Describe(nameof(Foo.Bar), () =>
            {
                Describe("when x is y", () =>
                {
                });
            });
        });
    }
}
```

<a id="its"></a><a href="#jazsharp" class="to-top">To top</a>
#### Its (tests)

Now that you\'ve specified the scopes for the class, method and (optionally) scenarios
for your test, it is time to start defining the test itself. This is done by using the
`It` methods. Each test's description should begin with "should" since this yields
readable test descriptions.

```C#
class FooSpec : Spec
{
    public FooSpec()
    {
        Describe<Foo>(() =>
        {
            Describe(nameof(Foo.Bar), () =>
            {
                It("should initialize the flux capacitor.", () =>
                {
                    ... // test logic here
                });
            });
        });
    }
}
```

The above test will yield the following test description:

`Foo Bar should initialize the flux capacitor.`

<a id="beforeAfterEach"></a><a href="#jazsharp" class="to-top">To top</a>
#### BeforeEach and AfterEach

JazSharp allows you to specify logic that will execute before and after each test. This
logic is scoped to the Describe in which it is defined. See the below code for an illustration
of how the scoping works.

```C#
class FooSpec : Spec
{
    public FooSpec()
    {
        Describe<Foo>(() =>
        {
            BeforeEach(() =>        // before each 1
            {
                ...
            });

            Describe(nameof(Foo.Bar), () =>
            {
                It("should initialize the flux capacitor.", () => 
                {
                    // before each 1 will be run
                    // before each 2 will not be run
                    ... // test logic here
                });
            });

            Describe("something else", () =>
            {
                BeforeEach(() =>    // before each 2
                {
                    ...
                });
            });
        });
    }
}
```

Before and After Each can be defined anywhere inside the Describe - before each doesn't have
to be declared before Its. It is recommended to have BeforeEach blocks at the start of a
Descibe and AfterEach at the end in order to make the test code easier to follow.

Expectations (JazSharp's equivalent of Asserts - covered in a future section) can also be
specified in Before and After Each blocks. This can allow you to have expectations shared
among multiple tests.

<a id="excludingAndFocusing"></a><a href="#jazsharp" class="to-top">To top</a>
#### Excluding and focusing

Sometimes there are tests cannot be safely re-run, are intentionally broken or are simply
not relevant to pending work. In this case, it may be desirable to exclude those tests.
Excluding is a simple matter of prefixing a `Describe` or `It` call with an "x". `xDescribe`
and `xIt` result in a test being listed but running unit tests will result in a skipped
status.

There may be other times, such as when working in a specific area or updating specific unit
tests where explicity focusing those tests may be convenient. While the Test Explorer does
make running specific tests easy, it is also possible to "focus" tests in JazSharp. Tests can
be focused by adding an "f" prefix to a `Describe` or `It` call. If any tests are focused,
only focused tests will be executed.

<a id="output"></a><a href="#jazsharp" class="to-top">To top</a>
#### Custom Output

A test can add to the output log for a test by writing to the output `StringBuilder` in
the current test context:

```C#
Jaz.CurrentTest.Output.AppendLine("This will appear in the test output.");
```

<a id="spying"></a><a href="#jazsharp" class="to-top">To top</a>
### Spying

<a id="whatAreSpies"></a>
#### What are spies

Spies are similar to mocks except that they are applied on a per-method basis. A spy is an
alternative implementation of a method which records calls made to the method and allows the
test to specify alternative behaviours of the methods.

<a id="behaviours"></a><a href="#jazsharp" class="to-top">To top</a>
#### Behaviours

You can spy on a method by using the `Jaz.SpyOn` method. Once spied on, a method will not
get executed. If the method is a function, the default value will be returned. If the method
has Out parameters, these too will be defaulted.

```C#
var value = "test";
var spy = Jaz.SpyOn(value, nameof(value.ToString));
var result = value.ToString(); // result is null
```

You can also spy on static methods.

```C#
var spy = Jaz.SpyOn(typeof(string), nameof(string.IsNullOrEmpty));
var result = string.IsNullOrEmpty(null); // result is false
```

If a method has multiple overrides, you can specify the parameters of the overload you want
to spy on.

```C#
var spy = Jaz.SpyOn(typeof(int), nameof(int.Parse), new[] { typeof(string) });
var result = int.Parse("1"); // result is 0
```

To keep the spy and execute the method's original implementation, use the `CallThrough` method.

```C#
var value = "test";
var spy = Jaz.SpyOn(value, nameof(value.ToString)).And.CallThrough();
var result = value.ToString(); // result is "test"
```

To instead return another value from the function, use the `ReturnValue` method.

```C#
var value = "test";
var spy = Jaz.SpyOn(value, nameof(value.ToString)).And.ReturnValue("other");
var result = value.ToString(); // result is "other"
```

You can also specify a sequence of return values that each subsequent call to the method
will return. Once the sequence runs out, a `JazSpyException` will be thrown.

```C#
var value = "test";
var spy = Jaz.SpyOn(value, nameof(value.ToString)).And.ReturnValues("other", "value");
var result = value.ToString(); // result is "other"
result = value.ToString(); // result is "value"
value.ToString(); // throws exception
```

Alternatively, you can specify that the method should throw an exception. You can specify
the exception either by passing in the instance:

```C#
var value = "test";
var exception = new InvalidOperationException();
var spy = Jaz.SpyOn(value, nameof(value.ToString)).And.Throw(exception);
value.ToString(); // throws invalid operation exception
```

or by specifying an exception type:

```C#
var value = "test";
var spy = Jaz.SpyOn(value, nameof(value.ToString)).And.Throw<InvalidOperationException>();
value.ToString(); // throws invalid operation exception
```

You can also change the parameters used for a call through by using `ChangeParameterBefore`
one or more times.

```C#
var spy =
    Jaz.SpyOn(typeof(int), nameof(int.Parse), new[] { typeof(string) })
        .And
        .ChangeParameterBefore("s", "3")
        .CallThrough();

var result = int.Parse("5"); // result is 3
```

Similarly, you can set the value of an Out or Ref parameter by using `ThenChangeParameter`
one or more times.

```C#
var spy =
    Jaz.SpyOn(typeof(int), nameof(int.Parse), new[] { typeof(string) })
        .And
        .DoNothing()
        .ThenChangeParameter("result", 9);

var result = int.Parse("5", out var parsedValue); // result is false, parsedValue is 9
```

`DoNothing` behaves the same as a default spy. The method exists solely for the purpose
of calling `ThenChangeParameter`.

<a id="sequences"></a><a href="#jazsharp" class="to-top">To top</a>
#### Sequences and Quantifiers

Spy behaviours can also be defined in a sequence. You got a taste for this when using
`ReturnValues`. The following code:

```C#
var value = "test";
var spy = Jaz.SpyOn(value, nameof(value.ToString)).And.ReturnValues("a", "b", "c");
```

could also be written as:

```C#
var value = "test";
var spy =
    Jaz.SpyOn(value, nameof(value.ToString))
        .And.ReturnValue("a")
        .Then.ReturnValue("b")
        .Then.ReturnValue("c");
```

All of the spy behaviours can be specified in a sequence.

In addition to being able to specify behaviours in a sequence, you can also specify
how many times each behaviour is used before moving on to the next behaviour.

The above code could also be extended out to the following code:

```C#
var value = "test";
var spy =
    Jaz.SpyOn(value, nameof(value.ToString))
        .And.ReturnValue("a").Once()
        .Then.ReturnValue("b").Once()
        .Then.ReturnValue("c").Once();
```

The available quantifiers are:

- `Once()`: the behaviour executes once.
- `Twice()`: the behaviour executes twice.
- `Times(x)`: the behaviour executes a given number of times.
- `Forever()`: the behaviour executes forever.

<a id="propertySpies"></a><a href="#jazsharp" class="to-top">To top</a>
#### Properties

In addition to spying on methods, you can also spy on properties. The following
will create a spy on the Get and/or Set of methods of a property.

```C#
var array = new int[0];
var propertySpy = Jaz.SpyOnProperty(array, nameof(array.Length));
```

The spies for the Get and Set methods can be configured like method spies:

```C#
var array = new int[0];
var propertySpy =
    Jaz.SpyOnProperty(array, nameof(array.Length))
        .Getter.And.ReturnValue(5);
var length = array.Length; // length is 5
```

If the property does not have a Get method or it does not have a Set method then
`Getter` or `Setter` will be null respectively.

```C#
var array = new int[0];
var propertySpy = Jaz.SpyOnProperty(array, nameof(array.Length)); // propertySpy.Setter is null
```

<a id="createSpy"></a><a href="#jazsharp" class="to-top">To top</a>
#### Create Spy

There are times where a spy in the form of a delegate is needed. These can be created
by calling the provided `Jaz.CreateSpy` and `Jaz.CreateSpyFunc` methods. These are
useful for testing events and delegates passed in as parameters.

```C#
var button = new Button();
button.Click += Jaz.CreateSpy<object, EventArgs>(out var spy);
```

```C#
var list = new List<int>();
list.Where(Jaz.CreateSpyFunc<int, boolean>(out var spy));
```

<a id="calls"></a><a href="#jazsharp" class="to-top">To top</a>
#### Calls

The `Calls` property that is available on the Spy object allows the test to retrieve information
about each call to the spied on method. A case where this data is needed would be if the method
being tested passes in a callback to another method. You can then get the callback from
`Calls[i].Arguments[j]` and then test the behaviour of that callback.

<a id="spyLimits"></a><a href="#jazsharp" class="to-top">To top</a>
#### Limitations

There are a few limitations on which methods can be spied on. These limitations include:

- calls to base implementations of an override.

```C#
public override void ToString()
{
    return base.ToString(); // cannot be spied on
}
```

- Calls to value type instance methods

This limitation comes from the fact that the `this` parameter receives special treatment
for structs - it is passed by reference. This is done by the .net compiler so that methods
called on a struct can change the struct's state. If this wasn't done, any changes made in
a method would only affect a copy of the struct and thus changes would be lost.

This limitation may be removed in a future version of JazSharp.

```C#
var now = DateTime.Now;
now.ToString(); // cannot by spied on
```

- Calls to possible value type instance methods

An extension to the previous limitation, if you have a generic parameter which does not have
a `class` constraint on it, the .net compiler will pass the `this` parameter by reference
to make sure that if the type parameter is a struct, it will execute correctly.

```C#
public void Foo<TBar>(TBar bar)
{
    bar.ToString(); // cannot be spied on
}
```

The workaround for this limitation is to specify the `class` constraint on any generic
parameters you know will be class types.

```C#
public void Foo<TBar>(TBar bar)
    where TBar : class
{
    bar.ToString(); // CAN be spied on
}
```

- Calls with more than 16 parameters

Long story short, each parameter count needs to be explicitly handled by the framework and
at this time only 0 to 16 parameters are implemented. This may be extended later to support
methods with more than parameters.

- Executing tests in Release mode

There are outstanding issues when running JazSharp tests in a Release configuration. Only a
handful of scenarios are affected by this issue and these should be resolved in a future
minor version.

These are the known limitations of the current version of JazSharp. Many different kinds of
method calls have been tested but other, more obscure scenarios may have been missed. The
changes are low but if you encounter an `InvalidProgramException` or other issue then
please report it.

<a id="expects"></a><a href="#jazsharp" class="to-top">To top</a>
### Expectations (asserts)

JazSharp provides a sizable set of methods that can be used to check if the test was successful
or not. In xUnit, NUnit and MSTest this is called an Assert. In Jasmine and JazSharp this is
instead referred to as an Expect or expectation.

If an expectation fails, a `JazExpectationException` is thrown with a descriptive message. This
exception should not be caught since it is used when detecting failed tests.

All expectations can be reversed by using `Not` to reverse the check. For example:

```C#
var spy = Jaz.SpyOnProperty(typeof(DateTime), nameof(DateTime.Now)).Getter;
var now = DateTime.Now;
Expect(spy).ToHaveBeenCalled(); // passes
Expect(spy).Not.ToHaveBeenCalled(); // fails
```

The message in the `JazExpectationException` is different based on whether or not the check
was inverted.

<a id="spyExpects"></a><a href="#jazsharp" class="to-top">To top</a>
#### Spy Expectations

One of the three supported targets for an expectation is a spy. Spies can be checked in the
following ways:

- `ToHaveBeenCalled()`: checks whether or not a spied on method was called. Does not check
parameters nor call count.

```C#
var spy = Jaz.SpyOnProperty(typeof(DateTime), nameof(DateTime.Now)).Getter;
var now = DateTime.Now;
Expect(spy).ToHaveBeenCalled(); // passes
```

- `ToHaveBeenCalledTimes(x)`: checks whether or not a spied on method was called an expected
number of times.

```C#
var spy = Jaz.SpyOnProperty(typeof(DateTime), nameof(DateTime.Now)).Getter;
var now = DateTime.Now;
now = DateTime.Now;
now = DateTime.Now;
Expect(spy).ToHaveBeenCalledTimes(3); // passes
```

- `ToHaveBeenCalledWith(...)`: does a deep equality comparsion checking if the given set of
parameters matched any of the method calls that were made to the method.

```C#
var value = "a;b;c";
var spy = Jaz.SpyOn(value, nameof(string.Split), new[] { typeof(char[]) });
value.Split(new[] { ';' });
Expect(spy).ToHaveBeenCalledWith(new[] { ';' }); // passes
```

<a id="callExpects"></a><a href="#jazsharp" class="to-top">To top</a>
#### Call Expectations

A method call be wrapped in `Expect` in order to allow an expected exception throw to be
caught. This exception can be be checked by calling `ToThrow`.

```C#
var exception =
    Expect(() => throw new InvalidOperationException())
        .ToThrow<InvalidOperationException>(); // passes
```

`ToThrow` will fail if the exception inherits from the expected type:

```C#
Expect(() => throw new InvalidOperationException()).ToThrow<Exception>(); // fails
```

Note: even if `ToThrow` is not called, the original exception will be caught and thus
suppressed so only use a Call Expectation when checking for an expected exception.

<a id="valueExpects"></a><a href="#jazsharp" class="to-top">To top</a>
#### Value Expectations

This is where the bulk of the expectation logic lies. The following checks are provided:

- `ToEqual(x)`: performs a deep equality check comparing the two given values.

```C#
var value1 = new { x = 1, y = 2 };
Expect(value1).ToEqual(new { x = 1, y = 2 }); // passes
```

- `ToBe(x)`: checks for an exact match between the two values.

```C#
Expect("abc").ToBe("abc"); // passes
Expect(2).ToBe(2); // passes
Expect(new object()).ToBe(new object()); // fails - different reference
```

- `ToBeTrue()`: checks that the value is exactly `true`.

```C#
Expect(true).ToBeTrue(); // passes
Expect("true").ToBeTrue(); // fails
Expect(false).ToBeTrue(); // fails
```

- `ToBeFalse()`: checks that the value is exactly `false`.

```C#
Expect(false).ToBeFalse(); // passes
Expect("false").ToBeFalse(); // fails
Expect(true).ToBeFalse(); // fails
```

- `ToBeDefault()`: checks that the value is exactly `default`.

```C#
Expect(0).ToBeDefault(); // passes;
Expect(false).ToBeDefault(); // passes;
Expect((string)null).ToBeDefault(); // passes;
```

- `ToBeEmpty()`: checks that the value is an empty string or enumerable.

```C#
Expect("").ToBeEmpty(); // passes
Expect(new int[0]); // passes
```

- `ToBeBetween(x, y)`: checks that the value is between x and y.

```C#
Expect(5).ToBeBetween(3, 7); // passes
Expect(new DateTime(2010, 10, 10)).ToBeBetween(new DateTime(2009, 9, 9), new DateTime(2011, 11, 11));
```

This will work for any value that implements `IComparable<T>`.

- `ToBeLessThan(x)`: checks that the value is less than x.

```C#
Expect(5).ToBeLessThan(6); // passes
```

This will work for any value that implements `IComparable<T>`.

- `ToBeGreaterThan(x)`: checks that the value is greater than x.

```C#
Expect(5).ToBeGreaterThan(4); // passes
```

This will work for any value that implements `IComparable<T>`.

- `ToBeLessThanOrEqualTo(x)`: checks that the value is less than or equal to x.

```C#
Expect(5).ToBeLessThanOrEqualTo(6); // passes
Expect(5).ToBeLessThanOrEqualTo(5); // passes
```

This will work for any value that implements `IComparable<T>`.

- `ToBeGreaterThanOrEqualTo(x)`: checks that the value is greater than or equal to x.

```C#
Expect(5).ToBeLessThanOrEqualTo(4); // passes
Expect(5).ToBeLessThanOrEqualTo(5); // passes
```

This will work for any value that implements `IComparable<T>`.

- `ToMatch(x)`: checks that a string value matches the given Regular Expression.

```C#
Expect("a1122").ToMatch("^a[12]{4}$"); // passes
Expect("A1122").ToMatch(new Regex("^a[12]{4}$", RegexOptions.IgnoreCase)); // passes
```

- `ToContain(x)`: checks that the value contains the given subset of data.

Similar to `ToEquals` except that contains only requires x to be a subset of the
items and properties in the value. This is recursive so if the items in the list
are objects, the x item only needs to contain a subset of the properties.

There is also an overload for `ToContain` that checks if a string value contains the
given substring.

```C#
Expect(new[] { "a", "b", "c" }).ToContain(new[] { "c", "b" }); // passes
Expect("abc").ToContain("bc"); // passes
Expect("abc").ToContain("cb"); // fails
Expect(new { x = 1, y = 2, z = 3 }).ToContain(new { y = 2 }); // passes
```

<a id="matchers"></a><a href="#jazsharp" class="to-top">To top</a>
#### Matchers

`ToEqual` and `ToContain` support a set of matchers which can be used in place of
values when doing a comparison. The available matchers are:

- `Jaz.Any()`: matches on anything except null.
- `Jaz.AnyOrNull()`: matches on absolutely anything, including null.
- `Jaz.Any<T>()`: matches on any value that is of the given type or inherits from
the given type.
- `Jaz.AnyOrNull<T>()`: matches on any value that is of the given type, inherits
from the given type or is null.
- `Jaz.InstanceOf<T>()`: matches on any value that is of type T. Inheriting types,
and null values are not matched.

```C#
Expect(foo).ToEqual(new { x = Jaz.Any<int>() });
```

<a id="developerGuide"></a><a href="#jazsharp" class="to-top">To top</a>
## Developer Guide

You can clone the repository using this link: https://github.com/Sellorio/JazSharp.git

<a id="buildAndRun"></a>
### Build and run

Simply open the JazSharp solution in the root of the repository and build it.

Once built, Visual Studio will automatically pick up the Test Adapter. At that
point you will be able to execute the automated tests contained in the
JazSharp.Tests project.

<a id="devMap"></a>
### Map

Comming soon: a description of the areas of the source code to help interested
parties navigate and understand the code and how it works.