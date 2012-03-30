#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using Machine.Specifications;
using WritingText;

// Feature: (is the folder I'm in)
// Scenario: FileSystemExceptions
//
// Given: a source file and a destination file that is already open
// When: copying from source to destination file
// Then: should throw System.IO.IOException



namespace FileSystemExceptions {
	class When_copying_from_source_to_destination_file : with.a_source_file_and_a_destination_file_that_is_already_open {
		Because copying_to_an_open_file = ShouldFail(() => { });

		It should_throw__System_IO_IOException__ = () => the_exception.ShouldBeOfType<System.IO.IOException>();
	}

	#region contexts
	namespace with {
		[Subject("with a source file and a destination file that is already open")]
		public abstract class a_source_file_and_a_destination_file_that_is_already_open : an_open_IFileManager_transaction {

			Establish context = () => {
			};
		}
	}
	#endregion
}
