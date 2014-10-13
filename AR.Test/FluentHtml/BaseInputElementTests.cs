﻿using System;
using System.Collections;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AR.Website.Utility.FluentHtml.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AR.Test.FluentHtml
{
    [TestClass]
    public class BaseInputElementTests : BaseElementTests
    {
        private ViewDataDictionary<TestModel> GetViewData(TestModel model)
        {
            ViewDataDictionary<TestModel> viewData = new ViewDataDictionary<TestModel>(model)
            {
                {"StringProperty", "StringValue"}
            };
            viewData.Model = new TestModel
            {
                IntegerProperty = 1,
                StringProperty = "StringValue"
            };

            return viewData;
        }

        [TestMethod]
        public void SetsElementIDNameAndValueFromModelExpression()
        {
            TestModel model = new TestModel
            {
                StringProperty = "StringValue"
            };

            ViewDataDictionary<TestModel> viewData = GetViewData(model);
            HtmlHelper<TestModel> htmlHelper = GetHtmlHelper(viewData);
            InputTextElement element = new InputTextElement(htmlHelper).For<TestModel>(x => x.StringProperty);
            Assert.AreEqual(@"<input id=""StringProperty"" name=""StringProperty"" type=""text"" value=""StringValue"" />", element.ToHtmlString());
        }

        public class TestModel
        {
            public int IntegerProperty
            {
                get;
                set;
            }

            public string StringProperty
            {
                get;
                set;
            }

            public ChildModel Child
            {
                get;
                set;
            }
        }

        public class ChildModel
        {
            public string ChildStringProperty
            {
                get;
                set;
            }
        }
    }
}