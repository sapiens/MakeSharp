using MakeSharp;


namespace Tests
{
    public class Generic
    {
        static Solution AnySolution()
        {
            return new Solution(@"..\..\..\MakeSharp.sln");
        }

        //[Fact]
        public void release_dir_with_no_offset()
        {
            var proj = new Project("MakeSharp",AnySolution());
            proj.ReleaseDirOffset = "net45";
            var asmp = proj.ReleasePathForAssembly();
            var other = proj.ReleasePathForAssembly("CavemanTools.dll");
        } 
    }
}