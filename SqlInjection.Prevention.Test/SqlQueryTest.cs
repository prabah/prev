using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlInjection.Prevention.IO.Borker;

namespace SqlInjection.Prevention.Test
{
    [TestClass]
    public class SqlQueryTest
    {
        [TestMethod]
        public void ReturnsSameQueryWhenCorrectQueryIsPresentWithAllFields()
        {
            var sqlQuery = "SELECT * FROM TbTest WHERE TestField = 1";
            var allowedFields = new List<string>() {"*"};
            var queryProcessor = new SqlQueryProcessor(allowedFields, "TbTest", sqlQuery);
            var validatedQuery = queryProcessor.GetQueryValidated();

            Assert.IsTrue(validatedQuery == sqlQuery);
        }

        [TestMethod]
        public void ReturnsSameQueryWhenCorrectQueryIsPresentSelectingWithApprovedFields()
        {
            var sqlQuery = "SELECT TestField1, TestField2 FROM TbTest WHERE TestField = 1";
            var allowedFields = new List<string>() { "*", "TestField1", "TestField2" };
            var queryProcessor = new SqlQueryProcessor(allowedFields, "TbTest", sqlQuery);
            var validatedQuery = queryProcessor.GetQueryValidated();

            Assert.IsTrue(validatedQuery == sqlQuery);
        }

        [TestMethod]
        public void ReturnsSameQueryWhenNotAllowedFieldsInTheQuery()
        {
            var sqlQuery = "SELECT TestField1, TestField2, TestField3 FROM TbTest WHERE TestField = 1";
            var expectedQuery = "SELECT TestField1, TestField2 FROM TbTest WHERE TestField = 1";
            var allowedFields = new List<string>() { "*", "TestField1", "TestField2" };
            var queryProcessor = new SqlQueryProcessor(allowedFields, "TbTest", sqlQuery);
            var validatedQuery = queryProcessor.GetQueryValidated();

            Assert.IsTrue(validatedQuery == expectedQuery);
        }

        [TestMethod]
        public void ReturnsCorrectWhenAdditionalParametersInWhereClause()
        {
            var sqlQuery = "SELECT TestField1, TestField2, TestField3 FROM TbTest WHERE TestField = 1 AND Testfield LIKE '%test%";
            var expectedQuery = "SELECT TestField1, TestField2 FROM TbTest WHERE TestField = 1";
            var allowedFields = new List<string>() { "*", "TestField1", "TestField2" };
            var queryProcessor = new SqlQueryProcessor(allowedFields, "TbTest", sqlQuery);
            var validatedQuery = queryProcessor.GetQueryValidated();

            Assert.IsTrue(validatedQuery == expectedQuery);
        }

        [TestMethod]
        public void ReturnsCorrectWhenWhereClauseHasNonEqualAsCondition()
        {
            var sqlQuery = "SELECT TestField1, TestField2, TestField3 FROM TbTest WHERE TestField LIKE '%test%";
            var expectedQuery = "SELECT TestField1, TestField2 FROM TbTest WHERE TestField = %test%";
            var allowedFields = new List<string>() { "*", "TestField1", "TestField2" };
            var queryProcessor = new SqlQueryProcessor(allowedFields, "TbTest", sqlQuery);
            var validatedQuery = queryProcessor.GetQueryValidated();

            Assert.IsTrue(validatedQuery == expectedQuery);
        }
    }
}
