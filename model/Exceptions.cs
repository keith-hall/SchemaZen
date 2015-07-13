﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SchemaZen.model {
	public class BatchSqlFileException : Exception {
		public List<SqlFileException> Exceptions { get; set; }
	}

	public class SqlBatchException : Exception {
		private readonly int lineNumber;

		private readonly string message;

	  private readonly string sql;

		public SqlBatchException(SqlException ex, int prevLinesInBatch, string sql)
			: base("", ex) {
			lineNumber = ex.LineNumber + prevLinesInBatch;
			message = ex.Message;
		  this.sql = sql;
		}

		public int LineNumber {
			get { return lineNumber; }
		}

		public override string Message {
			get { return message + " while executing " + sql; }
		}

	  public string SQL {
	    get {
	      return sql;
	    }
	  }
	}

	public class SqlFileException : SqlBatchException {
		private readonly string fileName;

    public SqlFileException(string fileName, SqlBatchException ex)
			: base((SqlException) ex.InnerException, ex.LineNumber - 1, ex.SQL) {
			this.fileName = fileName;
		}

		public string FileName {
			get { return fileName; }
		}
	}

	public class DataFileException : Exception {
		private readonly string _fileName;
		private readonly int _lineNumber;
		private readonly string _message;

		public DataFileException(string message, string fileName, int lineNumber) {
			_message = message;
			_fileName = fileName;
			_lineNumber = lineNumber;
		}

		public override string Message {
			get { return _message; }
		}

		public string FileName {
			get { return _fileName; }
		}

		public int LineNumber {
			get { return _lineNumber; }
		}
	}

	public class DataException : Exception {
		private readonly int _lineNumber;
		private readonly string _message;

		public DataException(string message, int lineNumber) {
			_message = message;
			_lineNumber = lineNumber;
		}

		public override string Message {
			get { return _message; }
		}

		public int LineNumber {
			get { return _lineNumber; }
		}
	}
}
