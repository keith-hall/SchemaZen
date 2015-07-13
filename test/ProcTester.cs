﻿using NUnit.Framework;
using SchemaZen.model;

namespace SchemaZen.test {
	[TestFixture]
	public class ProcTester {
		[Test]
		public void TestScript() {
			var t = new Table("dbo", "Address");
			t.Columns.Add(new Column("id", "int", false, null));
			t.Columns.Add(new Column("street", "varchar", 50, false, null));
			t.Columns.Add(new Column("city", "varchar", 50, false, null));
			t.Columns.Add(new Column("state", "char", 2, false, null));
			t.Columns.Add(new Column("zip", "char", 5, false, null));
			t.Constraints.Add(new Constraint("PK_Address", "PRIMARY KEY", "id"));

			var getAddress = new Routine("dbo", "GetAddress");
			getAddress.Text = @"
CREATE PROCEDURE [dbo].[GetAddress]
	@id int
AS
	select * from Address where id = @id
";

			TestHelper.ExecSql(t.ScriptCreate(), "");
			TestHelper.ExecBatchSql(getAddress.ScriptCreate(null) + "\nGO", "");
			TestHelper.ExecSql("drop table [dbo].[Address]", "");
			TestHelper.ExecSql("drop procedure [dbo].[GetAddress]", "");
		}

		[Test]
		public void TestScriptWrongName()
		{
			var t = new Table("dbo", "Address");
			t.Columns.Add(new Column("id", "int", false, null));
			t.Columns.Add(new Column("street", "varchar", 50, false, null));
			t.Columns.Add(new Column("city", "varchar", 50, false, null));
			t.Columns.Add(new Column("state", "char", 2, false, null));
			t.Columns.Add(new Column("zip", "char", 5, false, null));
			t.Constraints.Add(new Constraint("PK_Address", "PRIMARY KEY", "id"));

			TestHelper.ExecSql(t.ScriptCreate(), "");

			var baseText = @"--example of routine that has been renamed since creation
CREATE PROCEDURE {0}
	@id int
AS
	select * from Address where id = @id
";
			var getAddress = new Routine("dbo", "GetAddress");
			getAddress.Text = string.Format(baseText, "[dbo].[NamedDifferently]");

			var script = getAddress.ScriptCreate(null);
			Assert.IsTrue(script.Contains(string.Format(baseText, "[dbo].[GetAddress]")));
			TestHelper.ExecBatchSql(script + "\nGO", "");

			TestHelper.ExecSql("drop procedure [dbo].[GetAddress]", "");

			
			getAddress = new Routine("dbo", "GetAddress");
			baseText = baseText.Replace("--", "/*").Replace("CREATE", "*/CREATE");
			getAddress.Text = string.Format(baseText, "dbo.NamedDifferently");

			script = getAddress.ScriptCreate(null);
			Assert.IsTrue(script.Contains(string.Format(baseText, "[dbo].[GetAddress]")));
			TestHelper.ExecBatchSql(script + "\nGO", "");

			TestHelper.ExecSql("drop table [dbo].[Address]", "");
			
		}

		[Test]
		public void TestScriptAlter()
		{
			var t = new Table("dbo", "Address");
			t.Columns.Add(new Column("id", "int", false, null));
			t.Columns.Add(new Column("street", "varchar", 50, false, null));
			t.Columns.Add(new Column("city", "varchar", 50, false, null));
			t.Columns.Add(new Column("state", "char", 2, false, null));
			t.Columns.Add(new Column("zip", "char", 5, false, null));
			t.Constraints.Add(new Constraint("PK_Address", "PRIMARY KEY", "id"));

			TestHelper.ExecSql(t.ScriptCreate(), "");

			const string baseText = @"
{0} PROCEDURE [dbo].[GetAddress]
	@id int
AS
	select * from Address where id = @id
";
			var getAddress = new Routine("dbo", "GetAddress");
			getAddress.Text = string.Format(baseText, "CREATE");

			var script = getAddress.ScriptCreate(null);
			TestHelper.ExecBatchSql(script + "\nGO", "");

			var alter = getAddress.ScriptAlter(null);
			var compare = string.Format(baseText, "ALTER");
			Assert.IsTrue(alter.Contains(compare));
			TestHelper.ExecBatchSql(alter + "\nGO", "");

			TestHelper.ExecSql("drop procedure [dbo].[GetAddress]", "");
			TestHelper.ExecSql("drop table [dbo].[Address]", "");

		}
	}
}
