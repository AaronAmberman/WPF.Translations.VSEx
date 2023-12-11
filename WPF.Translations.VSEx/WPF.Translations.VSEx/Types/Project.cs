namespace WPF.Translations.VSEx.Types
{
    public class Project : SolutionItem
    {
        #region Properties

        public bool IsLoaded { get; set; }

        #endregion

        #region Constructors

        public Project()
        {
        }

        public Project(Community.VisualStudio.Toolkit.Project project)
        {
            IsLoaded = project.IsLoaded;
            Name = project.Name;
            Text = project.Text;
            FullPath = project.FullPath;
            Parent = GetParent(project);
            Children = GetChildren(project);
        }

        #endregion
    }
}
