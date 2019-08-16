# JazSharp

## Contents

- [Introduction](#intro)
- [User Guide](#userGuide)
- - [Installation](#installation)
- - [Creating tests](#creatingTests)
- - - [Creating the test class](#testClass)
- - - [Describes (test scopes)](#describes)
- - - [Its (tests)](#its)
- - - [BeforeEach and AfterEach](#beforeAfterEach)
- - [Spying](#spying)
- - - [What are spies?](#whatAreSpies)
- - - [Methods](#methodSpies)
- - - [Properties](#propertySpies)
- - - [Limitations](#spyLimits)
- - [Expectations (asserts)](#expects)
- - - [Spy Expectations](#spyExpects)
- - - [Call Expectations](#callExpects)
- - - [Value Expectations](#valueExpects)
- [Developer Guide](#developerGuide)
- - [Build and run](#buildAndRun)
- - [Map](#devMap)

## Introduction
<a id="intro"></a>

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

## User Guide
<a id="userGuide"></a>

### Installation
<a id="installation"></a>

To start using JazSharp, all you have to do is install the `JazSharp` and `JazSharp.TestAdapter` nuget packages
from nuget.org. The former is the core of the framework and the latter enables the framework to work with
Visual Studio\'s Test Explorer as well as allowing tests to be executed by calling `dotnet test`.

### Creating tests
<a id="creatingTests"></a>

#### Creating the test class
<a id="testClass"></a>

A test class in JazSharp is any class that inherits from `JazSharp.Spec`. The class does not need to be public.
Unlike the common practise in JavaScript (when using Jasmine), it is recommended to place test classes in a
dedicated unit test assembly (as is popular when unit testing in .Net).

An empty test class would look something like this:

```
class FooSpec : Spec
{
	public FooSpec()
	{
	}
}
```

#### Describes (test scopes)
<a id="describes"></a>

All tests need to be grouped into one or more levels of Describes. The first Describe should almost always be
used to state which class is being tested.

```
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

```
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

```
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

```
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

#### Its (tests)
<a id="its"></a>

Now that you\'ve specified the scopes for the class, method and (optionally) scenarios
for your test, it is time to start defining the test itself. This is done by using the
`It` methods. Each test's description should begin with "should" since this yields
readable test descriptions.

```
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

#### BeforeEach and AfterEach
<a id="beforeAfterEach"></a>

JazSharp allows you to specify logic that will execute before and after each test. This
logic is scoped to the Describe in which it is defined. See the below code for an illustration
of how the scoping works.

```
class FooSpec : Spec
{
	public FooSpec()
	{
		Describe<Foo>(() =>
		{
			BeforeEach(() =>		// before each 1
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
				BeforeEach(() =>	// before each 2
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

### Spying
<a id="spying"></a>


#### What are spies
<a id="whatAreSpies"></a>

#### Methods
<a id="methodSpies"></a>

#### Properties
<a id="propertySpies"></a>

#### Limitations
<a id="spyLimits"></a>

### Expectations (asserts)
<a id="expects"></a>

## Developer Guide
<a id="developerGuide"></a>

### Build and run
<a id="buildAndRun"></a>

### Map
<a id="devMap"></a>