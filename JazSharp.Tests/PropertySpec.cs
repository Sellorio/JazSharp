using System;

namespace JazSharp.Tests
{
    class PropertySpec : Spec
    {
        public PropertySpec()
        {
            Describe("Instance Property", () =>
            {
                TestSubject testSubject = null;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                });

                Describe("with get and set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(testSubject.GetSetProp).ToBe("123");
                        testSubject.GetSetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp));
                        testSubject.GetSetProp = "test.";

                        Expect(testSubject.GetSetProp).ToBeDefault();
                        Expect(testSubject.Value).ToBe("123");
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp)).Getter.And.ReturnValue("test.");
                        Expect(testSubject.GetSetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp));
                        propertySpy.Getter.And.CallThrough();
                        propertySpy.Setter.And.CallThrough();

                        Expect(testSubject.GetSetProp).ToBe("123");
                        testSubject.GetSetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp));
                        propertySpy.Getter.And.Throw<TestException>();
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => { var x = testSubject.GetSetProp; }).ToThrow<TestException>();
                        Expect(() => testSubject.GetSetProp = "test.").ToThrow<TestException>();
                    });
                });

                Describe("with only get", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(testSubject.GetProp).ToBe("123");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp));
                        Expect((object)propertySpy.Setter).ToBeDefault();
                        Expect(testSubject.GetProp).ToBeDefault();
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp)).Getter.And.ReturnValue("test.");
                        Expect(testSubject.GetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp));
                        propertySpy.Getter.And.CallThrough();

                        Expect(testSubject.GetProp).ToBe("123");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp));
                        propertySpy.Getter.And.Throw<TestException>();

                        Expect(() => { var x = testSubject.GetProp; }).ToThrow<TestException>();
                    });
                });

                Describe("with only set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        testSubject.SetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.SetProp));
                        testSubject.SetProp = "test.";

                        Expect((object)propertySpy.Getter).ToBeDefault();
                        Expect(testSubject.Value).ToBe("123");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.SetProp));
                        propertySpy.Setter.And.CallThrough();

                        testSubject.SetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.SetProp));
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => testSubject.SetProp = "test.").ToThrow<TestException>();
                    });
                });
            });

            Describe("Static Property", () =>
            {
                BeforeEach(() =>
                {
                    TestSubject.StaticValue = "abc";
                });

                Describe("with get and set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(TestSubject.StaticGetSetProp).ToBe("abc");
                        TestSubject.StaticGetSetProp = "test.";
                        Expect(TestSubject.StaticValue).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetSetProp));
                        TestSubject.StaticGetSetProp = "test.";

                        Expect(TestSubject.StaticGetSetProp).ToBeDefault();
                        Expect(TestSubject.StaticValue).ToBe("abc");
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetSetProp)).Getter.And.ReturnValue("test.");
                        Expect(TestSubject.StaticGetSetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetSetProp));
                        propertySpy.Getter.And.CallThrough();
                        propertySpy.Setter.And.CallThrough();

                        Expect(TestSubject.StaticGetSetProp).ToBe("abc");
                        TestSubject.StaticGetSetProp = "test.";
                        Expect(TestSubject.StaticValue).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetSetProp));
                        propertySpy.Getter.And.Throw<TestException>();
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => { var x = TestSubject.StaticGetSetProp; }).ToThrow<TestException>();
                        Expect(() => TestSubject.StaticGetSetProp = "test.").ToThrow<TestException>();
                    });
                });

                Describe("with only get", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(TestSubject.StaticGetProp).ToBe("abc");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetProp));
                        Expect((object)propertySpy.Setter).ToBeDefault();
                        Expect(TestSubject.StaticGetProp).ToBeDefault();
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetProp)).Getter.And.ReturnValue("test.");
                        Expect(TestSubject.StaticGetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetProp));
                        propertySpy.Getter.And.CallThrough();

                        Expect(TestSubject.StaticGetProp).ToBe("abc");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticGetProp));
                        propertySpy.Getter.And.Throw<TestException>();

                        Expect(() => { var x = TestSubject.StaticGetProp; }).ToThrow<TestException>();
                    });
                });

                Describe("with only set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        TestSubject.StaticSetProp = "test.";
                        Expect(TestSubject.StaticValue).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticSetProp));
                        TestSubject.StaticSetProp = "test.";

                        Expect((object)propertySpy.Getter).ToBeDefault();
                        Expect(TestSubject.StaticValue).ToBe("abc");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticSetProp));
                        propertySpy.Setter.And.CallThrough();

                        TestSubject.StaticSetProp = "test.";
                        Expect(TestSubject.StaticValue).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject), nameof(TestSubject.StaticSetProp));
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => TestSubject.StaticSetProp = "test.").ToThrow<TestException>();
                    });
                });
            });

            Describe("Generic Instance Property", () =>
            {
                TestSubject<string> testSubject = null;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject<string>();
                    testSubject.Value = "123";
                });

                Describe("with get and set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(testSubject.GetSetProp).ToBe("123");
                        testSubject.GetSetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp));
                        testSubject.GetSetProp = "test.";

                        Expect(testSubject.GetSetProp).ToBeDefault();
                        Expect(testSubject.Value).ToBe("123");
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp)).Getter.And.ReturnValue("test.");
                        Expect(testSubject.GetSetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp));
                        propertySpy.Getter.And.CallThrough();
                        propertySpy.Setter.And.CallThrough();

                        Expect(testSubject.GetSetProp).ToBe("123");
                        testSubject.GetSetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetSetProp));
                        propertySpy.Getter.And.Throw<TestException>();
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => { var x = testSubject.GetSetProp; }).ToThrow<TestException>();
                        Expect(() => testSubject.GetSetProp = "test.").ToThrow<TestException>();
                    });
                });

                Describe("with only get", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(testSubject.GetProp).ToBe("123");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp));
                        Expect((object)propertySpy.Setter).ToBeDefault();
                        Expect(testSubject.GetProp).ToBeDefault();
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp)).Getter.And.ReturnValue("test.");
                        Expect(testSubject.GetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp));
                        propertySpy.Getter.And.CallThrough();

                        Expect(testSubject.GetProp).ToBe("123");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.GetProp));
                        propertySpy.Getter.And.Throw<TestException>();

                        Expect(() => { var x = testSubject.GetProp; }).ToThrow<TestException>();
                    });
                });

                Describe("with only set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        testSubject.SetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.SetProp));
                        testSubject.SetProp = "test.";

                        Expect((object)propertySpy.Getter).ToBeDefault();
                        Expect(testSubject.Value).ToBe("123");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.SetProp));
                        propertySpy.Setter.And.CallThrough();

                        testSubject.SetProp = "test.";
                        Expect(testSubject.Value).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(testSubject, nameof(testSubject.SetProp));
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => testSubject.SetProp = "test.").ToThrow<TestException>();
                    });
                });
            });

            Describe("Generic Static Property", () =>
            {
                BeforeEach(() =>
                {
                    TestSubject<string>.StaticValue = "abc";
                });

                Describe("with get and set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(TestSubject<string>.StaticGetSetProp).ToBe("abc");
                        TestSubject<string>.StaticGetSetProp = "test.";
                        Expect(TestSubject<string>.StaticValue).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetSetProp));
                        TestSubject.StaticGetSetProp = "test.";

                        Expect(TestSubject<string>.StaticGetSetProp).ToBeDefault();
                        Expect(TestSubject<string>.StaticValue).ToBe("abc");
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetSetProp)).Getter.And.ReturnValue("test.");
                        Expect(TestSubject<string>.StaticGetSetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetSetProp));
                        propertySpy.Getter.And.CallThrough();
                        propertySpy.Setter.And.CallThrough();

                        Expect(TestSubject<string>.StaticGetSetProp).ToBe("abc");
                        TestSubject<string>.StaticGetSetProp = "test.";
                        Expect(TestSubject<string>.StaticValue).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetSetProp));
                        propertySpy.Getter.And.Throw<TestException>();
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => { var x = TestSubject<string>.StaticGetSetProp; }).ToThrow<TestException>();
                        Expect(() => TestSubject<string>.StaticGetSetProp = "test.").ToThrow<TestException>();
                    });
                });

                Describe("with only get", () =>
                {
                    It("should call through by default.", () =>
                    {
                        Expect(TestSubject<string>.StaticGetProp).ToBe("abc");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetProp));
                        Expect((object)propertySpy.Setter).ToBeDefault();
                        Expect(TestSubject<string>.StaticGetProp).ToBeDefault();
                    });

                    It("should return the configured value.", () =>
                    {
                        var spy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetProp)).Getter.And.ReturnValue("test.");
                        Expect(TestSubject<string>.StaticGetProp).ToBe("test.");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetProp));
                        propertySpy.Getter.And.CallThrough();

                        Expect(TestSubject<string>.StaticGetProp).ToBe("abc");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticGetProp));
                        propertySpy.Getter.And.Throw<TestException>();

                        Expect(() => { var x = TestSubject<string>.StaticGetProp; }).ToThrow<TestException>();
                    });
                });

                Describe("with only set", () =>
                {
                    It("should call through by default.", () =>
                    {
                        TestSubject<string>.StaticSetProp = "test.";
                        Expect(TestSubject<string>.StaticValue).ToBe("test.");
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticSetProp));
                        TestSubject<string>.StaticSetProp = "test.";

                        Expect((object)propertySpy.Getter).ToBeDefault();
                        Expect(TestSubject<string>.StaticValue).ToBe("abc");
                    });

                    It("should call through as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticSetProp));
                        propertySpy.Setter.And.CallThrough();

                        TestSubject<string>.StaticSetProp = "test.";
                        Expect(TestSubject<string>.StaticValue).ToBe("test.");
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var propertySpy = Jaz.SpyOnProperty(typeof(TestSubject<string>), nameof(TestSubject<string>.StaticSetProp));
                        propertySpy.Setter.And.Throw<TestException>();

                        Expect(() => TestSubject<string>.StaticSetProp = "test.").ToThrow<TestException>();
                    });
                });
            });
        }

        private class TestSubject
        {
            public static string StaticValue = "abc";
            public static string StaticGetProp => StaticValue;
            public static string StaticSetProp { set { StaticValue = value; } }
            public static string StaticGetSetProp
            {
                get => StaticValue;
                set { StaticValue = value; }
            }
            public string Value = "123";
            public string GetProp => Value;
            public string SetProp { set { Value = value; } }
            public string GetSetProp
            {
                get => Value;
                set { Value = value; }
            }
        }

        private class TestSubject<TValue>
        {
            public static TValue StaticValue;
            public static TValue StaticGetProp => StaticValue;
            public static TValue StaticSetProp { set { StaticValue = value; } }
            public static TValue StaticGetSetProp
            {
                get => StaticValue;
                set { StaticValue = value; }
            }
            public TValue Value;
            public TValue GetProp => Value;
            public TValue SetProp { set { Value = value; } }
            public TValue GetSetProp
            {
                get => Value;
                set { Value = value; }
            }
        }

        private class TestException : Exception
        {
        }
    }
}
