#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.IO.Transactions;
using System.Transactions;
using Machine.Specifications;

// Feature: (is the folder I'm in)
// Scenario: Transactions
//
// Given: a transactional file manager
// When: rolling back a read only file
// Then: throw a  TransactionException  



namespace Transactions {
	class When_rolling_back_a_read_only_file : with.a_transaction_having_a_monitored_file_marked_readonly {
		Because rolling_back_the_transaction = ShouldFail(() => scope.Dispose());

		It throw_a__TransactionException__ = () => the_exception.ShouldBeOfType<TransactionException>();
	}

	#region contexts
	namespace with {
		[Subject("with a transaction having a monitored file marked readonly")]
		public abstract class a_transaction_having_a_monitored_file_marked_readonly : ContextOf<IFileManager> {
			protected static TransactionScope scope;
			protected static string locked_file_path;
			protected static string content = "abc";

			Establish context = () => {
				subject = new TxFileManager();
				scope = new TransactionScope();
				locked_file_path = subject.GetTempFileName(".txt");
				subject.WriteAllText(locked_file_path, content);
                LockFile(locked_file_path);
			};

			Cleanup file =()=>
				{
					UnlockFile(locked_file_path);
					File.Delete(locked_file_path);
				};

			static void UnlockFile(string path)
			{
				var fi1 = new FileInfo(path);
				fi1.Attributes = FileAttributes.Normal;
			}

			static void LockFile(string path)
			{
				var fi1 = new FileInfo(path);
				fi1.Attributes = FileAttributes.ReadOnly;
			}
		}
	}
	#endregion
}
