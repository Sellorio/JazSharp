using JazSharp.Expectations;

namespace JazSharp.Tests.Expectations
{
    public class ValueExpectSpec : Spec
    {
        public ValueExpectSpec()
        {
            Describe<ValueExpect<object>>(() =>
            {
                Describe(nameof(ValueExpect<object>.ToBe), () =>
                {
                    It("should fail if value types are not equal.", () =>
                    {
                        Expect(() => Expect(5).ToBe(4)).ToThrow<JazExpectationException>();
                    });

                    It("should fail if strings are not equal.", () =>
                    {
                        Expect(() => Expect("a").ToBe("A")).ToThrow<JazExpectationException>();
                    });

                    It("should fail if classes are not same.", () =>
                    {
                        Expect(() => Expect(new object()).ToBe(new object())).ToThrow<JazExpectationException>();
                    });

                    It("should pass if value types are equal.", () =>
                    {
                        Expect(5).ToBe(5);
                    });

                    It("should pass if strings are equal.", () =>
                    {
                        Expect("a").ToBe("a ".Trim());
                    });

                    It("should pass if classes are same.", () =>
                    {
                        var instance = new object();
                        Expect(instance).ToBe(instance);
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass if value types are not equal.", () =>
                        {
                            Expect(5).Not.ToBe(4);
                        });

                        It("should pass if strings are not equal.", () =>
                        {
                            Expect("a").Not.ToBe("A");
                        });

                        It("should pass if classes are not same.", () =>
                        {
                            Expect(new object()).Not.ToBe(new object());
                        });

                        It("should fail if value types are equal.", () =>
                        {
                            Expect(() => Expect(5).ToBe(5)).Not.ToThrow<JazExpectationException>();
                        });

                        It("should fail if strings are equal.", () =>
                        {
                            Expect(() => Expect("a").ToBe("a ".Trim())).Not.ToThrow<JazExpectationException>();
                        });

                        It("should fail if classes are same.", () =>
                        {
                            var instance = new object();
                            Expect(() => Expect(instance).ToBe(instance)).Not.ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeBetween), () =>
                {
                    It("should fail if value is too low.", () =>
                    {
                        Expect(() => Expect(5).ToBeBetween(6, 9)).ToThrow<JazExpectationException>();
                    });

                    It("should fail if value is too high.", () =>
                    {
                        Expect(() => Expect(10).ToBeBetween(6, 9)).ToThrow<JazExpectationException>();
                    });

                    It("should pass if value is equal to min.", () =>
                    {
                        Expect(6).ToBeBetween(6, 9);
                    });

                    It("should pass if value is equal to max.", () =>
                    {
                        Expect(9).ToBeBetween(6, 9);
                    });

                    It("should pass if value is inside the range.", () =>
                    {
                        Expect(7).ToBeBetween(6, 9);
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass if value is too low.", () =>
                        {
                            Expect(5).Not.ToBeBetween(6, 9);
                        });

                        It("should pass if value is too high.", () =>
                        {
                            Expect(10).Not.ToBeBetween(6, 9);
                        });

                        It("should fail if value is equal to min.", () =>
                        {
                            Expect(() => Expect(6).Not.ToBeBetween(6, 9)).ToThrow<JazExpectationException>();
                        });

                        It("should fail if value is equal to max.", () =>
                        {
                            Expect(() => Expect(9).Not.ToBeBetween(6, 9)).ToThrow<JazExpectationException>();
                        });

                        It("should fail if value is inside the range.", () =>
                        {
                            Expect(() => Expect(7).Not.ToBeBetween(6, 9)).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeDefault), () =>
                {
                    It("should fail on an instance.", () =>
                    {
                        Expect(() => Expect(new object()).ToBeDefault()).ToThrow<JazExpectationException>();
                    });

                    It("should fail on non-default value.", () =>
                    {
                        Expect(() => Expect(1).ToBeDefault()).ToThrow<JazExpectationException>();
                    });

                    It("should pass on null.", () =>
                    {
                        Expect((object)null).ToBeDefault();
                    });

                    It("should pass on zero.", () =>
                    {
                        Expect(0).ToBeDefault();
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass on an instance.", () =>
                        {
                            Expect(new object()).Not.ToBeDefault();
                        });

                        It("should pass on non-default value.", () =>
                        {
                            Expect(1).Not.ToBeDefault();
                        });

                        It("should fail on null.", () =>
                        {
                            Expect(() => Expect((object)null).Not.ToBeDefault()).ToThrow<JazExpectationException>();
                        });

                        It("should pass on zero.", () =>
                        {
                            Expect(() => Expect(0).Not.ToBeDefault()).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeEmpty), () =>
                {
                    It("should fail on an array with items.", () =>
                    {
                        Expect(() => Expect(new[] { 1 }).ToBeEmpty()).ToThrow<JazExpectationException>();
                    });

                    It("should fail on a non-empty string.", () =>
                    {
                        Expect(() => Expect("a").ToBeEmpty()).ToThrow<JazExpectationException>();
                    });

                    It("should pass on an empty array.", () =>
                    {
                        Expect(new int[0]).ToBeEmpty();
                    });

                    It("should pass on an empty string.", () =>
                    {
                        Expect("").ToBeEmpty();
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass on an array with items.", () =>
                        {
                            Expect(new[] { 1 }).Not.ToBeEmpty();
                        });

                        It("should pass on a non-empty string.", () =>
                        {
                            Expect("a").Not.ToBeEmpty();
                        });

                        It("should fail on an empty array.", () =>
                        {
                            Expect(() => Expect(new int[0]).Not.ToBeEmpty()).ToThrow<JazExpectationException>();
                        });

                        It("should fail on an empty string.", () =>
                        {
                            Expect(() => Expect("").Not.ToBeEmpty()).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeFalse), () =>
                {
                    It("should fail on non-boolean values.", () =>
                    {
                        Expect(() => Expect(5).ToBeFalse()).ToThrow<JazExpectationException>();
                    });

                    It("should fail on null.", () =>
                    {
                        Expect(() => Expect((object)null).ToBeFalse()).ToThrow<JazExpectationException>();
                    });

                    It("should fail on true.", () =>
                    {
                        Expect(() => Expect(true).ToBeFalse()).ToThrow<JazExpectationException>();
                    });

                    It("should pass on false.", () =>
                    {
                        Expect(false).ToBeFalse();
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass on non-boolean values.", () =>
                        {
                            Expect(5).Not.ToBeFalse();
                        });

                        It("should pass on null.", () =>
                        {
                            Expect((object)null).Not.ToBeFalse();
                        });

                        It("should pass on true.", () =>
                        {
                            Expect(true).Not.ToBeFalse();
                        });

                        It("should fail on false.", () =>
                        {
                            Expect(() => Expect(false).Not.ToBeFalse()).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeTrue), () =>
                {
                    It("should fail on non-boolean values.", () =>
                    {
                        Expect(() => Expect(5).ToBeTrue()).ToThrow<JazExpectationException>();
                    });

                    It("should fail on null.", () =>
                    {
                        Expect(() => Expect((object)null).ToBeTrue()).ToThrow<JazExpectationException>();
                    });

                    It("should fail on false.", () =>
                    {
                        Expect(() => Expect(false).ToBeTrue()).ToThrow<JazExpectationException>();
                    });

                    It("should pass on true.", () =>
                    {
                        Expect(true).ToBeTrue();
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass on non-boolean values.", () =>
                        {
                            Expect(5).Not.ToBeTrue();
                        });

                        It("should pass on null.", () =>
                        {
                            Expect((object)null).Not.ToBeTrue();
                        });

                        It("should pass on false.", () =>
                        {
                            Expect(false).Not.ToBeTrue();
                        });

                        It("should fail on true.", () =>
                        {
                            Expect(() => Expect(true).Not.ToBeTrue()).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToEqual), () =>
                {
                    It("should fail on mismatching value types.", () =>
                    {
                        Expect(() => Expect(1).ToEqual(2)).ToThrow<JazExpectationException>();
                    });

                    It("should fail on different list lengths", () =>
                    {
                        Expect(() => Expect(new[] { 3 }).ToEqual(new int[0])).ToThrow<JazExpectationException>();
                    });

                    It("should fail on different list items.", () =>
                    {
                        Expect(() => Expect(new[] { 3 }).ToEqual(new[] { 1 })).ToThrow<JazExpectationException>();
                    });

                    It("should fail if expected has extra properties.", () =>
                    {
                        Expect(() => Expect(new { }).ToEqual(new { v = 0 })).ToThrow<JazExpectationException>();
                    });

                    It("should fail if expected is missing properties.", () =>
                    {
                        Expect(() => Expect(new { v = 0 }).ToEqual(new { })).ToThrow<JazExpectationException>();
                    });

                    It("should fail when comparing to Jaz.Any<T> of wrong type.", () =>
                    {
                        Expect(() => Expect(new { v = 0 }).ToEqual(new { v = Jaz.Any<double>() })).ToThrow<JazExpectationException>();
                    });

                    It("should fail when comparing to Jaz.InstanceOf<T> with inheriting value.", () =>
                    {
                        Expect(() => Expect(new { v = "" }).ToEqual(new { v = Jaz.InstanceOf<object>() })).ToThrow<JazExpectationException>();
                    });

                    It("should fail when comparing null to Jaz.InstanceOf<T> with nullable type.", () =>
                    {
                        Expect(() => Expect(new { v = (object)null }).ToEqual(new { v = Jaz.InstanceOf<object>() })).ToThrow<JazExpectationException>();
                    });

                    It("should pass on equivalent value types.", () =>
                    {
                        Expect(3).ToEqual(3);
                    });

                    It("should pass on equivalent reference types.", () =>
                    {
                        Expect("yes").ToEqual("yes ".Trim());
                    });

                    It("should pass on equivalent list lengths.", () =>
                    {
                        Expect(new int[0]).ToEqual(new object[0]);
                    });

                    It("should pass on equivalent list items.", () =>
                    {
                        Expect(new[] { "a", "b", "c" }).ToEqual(new[] { " a ".Trim(), " b ".Trim(), " c ".Trim() });
                    });

                    It("should pass on objects with the same properies with equivalent values.", () =>
                    {
                        Expect(new { v = "a" }).ToEqual(new { v = " a ".Trim() });
                    });

                    It("should pass when comparing to Jaz.Any<T>.", () =>
                    {
                        Expect(new { v = 1 }).ToEqual(new { v = Jaz.Any<int>() });
                    });

                    It("should pass when comparing to Jaz.Any.", () =>
                    {
                        Expect(new { v = 1 }).ToEqual(new { v = Jaz.Any() });
                    });

                    It("should pass when comparing to Jaz.AnyOrNull<T> when value is null and T is nullable.", () =>
                    {
                        Expect(new { v = (object)null }).ToEqual(new { v = Jaz.AnyOrNull<string>() });
                    });

                    It("should pass when comparing to Jaz.AnyOrNull when value is null.", () =>
                    {
                        Expect(new { v = (int?)null }).ToEqual(new { v = Jaz.AnyOrNull() });
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass on mismatching value types.", () =>
                        {
                            Expect(1).Not.ToEqual(2);
                        });

                        It("should pass on different list lengths", () =>
                        {
                            Expect(new[] { 3 }).Not.ToEqual(new int[0]);
                        });

                        It("should pass on different list items.", () =>
                        {
                            Expect(new[] { 3 }).Not.ToEqual(new[] { 1 });
                        });

                        It("should pass if expected has extra properties.", () =>
                        {
                            Expect(new { }).Not.ToEqual(new { v = 0 });
                        });

                        It("should pass if expected is missing properties.", () =>
                        {
                            Expect(new { v = 0 }).Not.ToEqual(new { });
                        });

                        It("should pass when comparing to Jaz.Any<T> of wrong type.", () =>
                        {
                            Expect(new { v = 0 }).Not.ToEqual(new { v = Jaz.Any<double>() });
                        });

                        It("should pass when comparing to Jaz.InstanceOf<T> with inheriting value.", () =>
                        {
                            Expect(new { v = "" }).Not.ToEqual(new { v = Jaz.InstanceOf<object>() });
                        });

                        It("should pass when comparing null to Jaz.InstanceOf<T> with nullable type.", () =>
                        {
                            Expect(new { v = (object)null }).Not.ToEqual(new { v = Jaz.InstanceOf<object>() });
                        });

                        It("should fail on equivalent value types.", () =>
                        {
                            Expect(() => Expect(3).Not.ToEqual(3)).ToThrow<JazExpectationException>();
                        });

                        It("should fail on equivalent reference types.", () =>
                        {
                            Expect(() => Expect("yes").Not.ToEqual("yes ".Trim())).ToThrow<JazExpectationException>();
                        });

                        It("should fail on equivalent list lengths.", () =>
                        {
                            Expect(() => Expect(new int[0]).Not.ToEqual(new object[0])).ToThrow<JazExpectationException>();
                        });

                        It("should fail on equivalent list items.", () =>
                        {
                            Expect(() => Expect(new[] { "a", "b", "c" }).Not.ToEqual(new[] { " a ".Trim(), " b ".Trim(), " c ".Trim() })).ToThrow<JazExpectationException>();
                        });

                        It("should fail on objects with the same properies with equivalent values.", () =>
                        {
                            Expect(() => Expect(new { v = "a" }).Not.ToEqual(new { v = " a ".Trim() })).ToThrow<JazExpectationException>();
                        });

                        It("should fail when comparing to Jaz.Any<T>.", () =>
                        {
                            Expect(() => Expect(new { v = 1 }).Not.ToEqual(new { v = Jaz.Any<int>() })).ToThrow<JazExpectationException>();
                        });

                        It("should fail when comparing to Jaz.Any.", () =>
                        {
                            Expect(() => Expect(new { v = 1 }).Not.ToEqual(new { v = Jaz.Any() })).ToThrow<JazExpectationException>();
                        });

                        It("should fail when comparing to Jaz.AnyOrNull<T> when value is null and T is nullable.", () =>
                        {
                            Expect(() => Expect(new { v = (object)null }).Not.ToEqual(new { v = Jaz.AnyOrNull<string>() })).ToThrow<JazExpectationException>();
                        });

                        It("should fail when comparing to Jaz.AnyOrNull when value is null.", () =>
                        {
                            Expect(() => Expect(new { v = (int?)null }).Not.ToEqual(new { v = Jaz.AnyOrNull() })).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeGreaterThan), () =>
                {
                    It("should fail if value is less than the given value.", () =>
                    {
                        Expect(() => Expect(3).ToBeGreaterThan(5)).ToThrow<JazExpectationException>();
                    });

                    It("should fail if value is equal to the given value.", () =>
                    {
                        Expect(() => Expect(5).ToBeGreaterThan(5)).ToThrow<JazExpectationException>();
                    });

                    It("should pass if value is greater than the given value.", () =>
                    {
                        Expect(7).ToBeGreaterThan(5);
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass if value is less than the given value.", () =>
                        {
                            Expect(3).Not.ToBeGreaterThan(5);
                        });

                        It("should pass if value is equal to the given value.", () =>
                        {
                            Expect(5).Not.ToBeGreaterThan(5);
                        });

                        It("should fail if value is greater than the given value.", () =>
                        {
                            Expect(() => Expect(7).Not.ToBeGreaterThan(5)).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeLessThan), () =>
                {
                    It("should fail if value is less than the given value.", () =>
                    {
                        Expect(() => Expect(7).ToBeLessThan(5)).ToThrow<JazExpectationException>();
                    });

                    It("should fail if value is equal to the given value.", () =>
                    {
                        Expect(() => Expect(5).ToBeLessThan(5)).ToThrow<JazExpectationException>();
                    });

                    It("should pass if value is greater than the given value.", () =>
                    {
                        Expect(3).ToBeLessThan(5);
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass if value is greater than the given value.", () =>
                        {
                            Expect(7).Not.ToBeLessThan(5);
                        });

                        It("should pass if value is equal to the given value.", () =>
                        {
                            Expect(5).Not.ToBeLessThan(5);
                        });

                        It("should fail if value is less than the given value.", () =>
                        {
                            Expect(() => Expect(3).Not.ToBeLessThan(5)).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeGreaterThanOrEqualTo), () =>
                {
                    It("should fail if value is less than the given value.", () =>
                    {
                        Expect(() => Expect(3).ToBeGreaterThanOrEqualTo(5)).ToThrow<JazExpectationException>();
                    });

                    It("should pass if value is equal to the given value.", () =>
                    {
                        Expect(5).ToBeGreaterThanOrEqualTo(5);
                    });

                    It("should pass if value is greater than the given value.", () =>
                    {
                        Expect(7).ToBeGreaterThanOrEqualTo(5);
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass if value is less than the given value.", () =>
                        {
                            Expect(3).Not.ToBeGreaterThanOrEqualTo(5);
                        });

                        It("should fail if value is equal to the given value.", () =>
                        {
                            Expect(() => Expect(5).Not.ToBeGreaterThanOrEqualTo(5)).ToThrow<JazExpectationException>();
                        });

                        It("should fail if value is greater than the given value.", () =>
                        {
                            Expect(() => Expect(7).Not.ToBeGreaterThanOrEqualTo(5)).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(ValueExpect<object>.ToBeLessThanOrEqualTo), () =>
                {
                    It("should fail if value is less than the given value.", () =>
                    {
                        Expect(() => Expect(7).ToBeLessThanOrEqualTo(5)).ToThrow<JazExpectationException>();
                    });

                    It("should pass if value is equal to the given value.", () =>
                    {
                        Expect(5).ToBeLessThanOrEqualTo(5);
                    });

                    It("should pass if value is greater than the given value.", () =>
                    {
                        Expect(3).ToBeLessThanOrEqualTo(5);
                    });

                    Describe("with Not", () =>
                    {
                        It("should pass if value is greater than the given value.", () =>
                        {
                            Expect(7).Not.ToBeLessThanOrEqualTo(5);
                        });

                        It("should fail if value is equal to the given value.", () =>
                        {
                            Expect(() => Expect(5).Not.ToBeLessThanOrEqualTo(5)).ToThrow<JazExpectationException>();
                        });

                        It("should fail if value is less than the given value.", () =>
                        {
                            Expect(() => Expect(3).Not.ToBeLessThanOrEqualTo(5)).ToThrow<JazExpectationException>();
                        });
                    });
                });
            });
        }
    }
}
