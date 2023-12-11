using Community.VisualStudio.Toolkit;
using System.Collections.Generic;
using System.Linq;

namespace WPF.Translations.VSEx.Types
{
    public class SolutionSnapshot
    {
        #region Properties

        public IDictionary<Project, List<SolutionItem>> Projects { get; set; } = new Dictionary<Project, List<SolutionItem>>();

        #endregion

        #region Constructors

        public SolutionSnapshot()
        {
        }

        #endregion

        #region Methods

        public async Task TakeSolutionSnapshotAsync()
        {
            IEnumerable<Community.VisualStudio.Toolkit.Project> projects = await VS.Solutions.GetAllProjectsAsync();

            foreach (Community.VisualStudio.Toolkit.Project project in projects)
            {
                Project p = new Project(project);

                List<SolutionItem> solutionItems = new List<SolutionItem>();

                foreach (Community.VisualStudio.Toolkit.SolutionItem item in project.Children)
                {
                    SolutionItem si = new SolutionItem(item);

                    solutionItems.Add(si);
                }

                Projects.Add(p, solutionItems);
            }
        }

        #endregion
    }
}
