#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using Machine.Specifications;
using Scaffold;

// Feature: (is the folder I'm in)
// Scenario: FileSystemExceptions
//
// Given: a source file and a destination file that is already open
// When: copying from source to destination file
// Then: should throw System.IO.IOException



namespace FileSystemExceptions {
	class When_copying_from_source_to_destination_file : with.a_source_file_and_a_destination_file_that_is_already_open {
		Because copying_to_an_open_file = ShouldFail(() => subject.Copy(source_file, opened_destination_file, false));

		It should_throw__System_IO_IOException__ = () => the_exception.ShouldBeOfType<System.IO.IOException>();
		It should_have_source_file_in_place =()=> File.Exists(source_file).ShouldBeTrue(); 
	}

	#region contexts
	namespace with {
		[Subject("with a source file and a destination file that is already open")]
		public abstract class a_source_file_and_a_destination_file_that_is_already_open : an_open_IFileManager_transaction {
			protected static string source_file, opened_destination_file;
			protected static string source_content = "123";
			protected static FileStream open_stream;

			Establish context = () => {
				source_file = subject.GetTempFileName();
				subject.WriteAllText(source_file, source_content);
				opened_destination_file = subject.GetTempFileName();
				open_stream = new FileStream(opened_destination_file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
			};

			Cleanup stream =()=> open_stream.Close();
		}
	}
	#endregion
}
