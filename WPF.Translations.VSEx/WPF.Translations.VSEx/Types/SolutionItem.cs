using System.Collections.Generic;
using System.Linq;

namespace WPF.Translations.VSEx.Types
{
    public class SolutionItem
    {
        #region Properties

        public string Name { get; set; }
        public string Text { get; set; }
        public string FullPath { get; set; }
        public SolutionItemType Type { get; set; }
        public SolutionItem Parent { get; set; }
        public List<SolutionItem> Children { get; set; } = new List<SolutionItem>();

        #endregion

        #region Condstructors

        public SolutionItem()
        {            
        }

        public SolutionItem(Community.VisualStudio.Toolkit.SolutionItem solutionItem)
        {
            FullPath = solutionItem.FullPath;
            Name = solutionItem.Name;
            Text = solutionItem.Text;
            Type = solutionItem.Type;
            Parent = GetParent(solutionItem);
            Children = GetChildren(solutionItem);
        }

        #endregion

        #region Methods

        public List<SolutionItem> GetChildren(Community.VisualStudio.Toolkit.SolutionItem solutionItem)
        {
            if (solutionItem.Children.Count() == 0) return new List<SolutionItem>();

            List<SolutionItem> items = new List<SolutionItem>();

            foreach (Community.VisualStudio.Toolkit.SolutionItem item in solutionItem.Children)
            {
                SolutionItem si = new SolutionItem
                {
                    FullPath = item.FullPath,
                    Name = item.Name,
                    Text = item.Text
                };

                if (item.Children.Count() > 0)
                    si.Children.AddRange(GetChildren(item));

                items.Add(si);
            }

            return items;
        }

        public SolutionItem GetParent(Community.VisualStudio.Toolkit.SolutionItem item)
        {
            if (item.Parent == null) return null;

            SolutionItem si = new SolutionItem
            {
                Name = item.Parent.Name,
                FullPath = item.Parent.FullPath,
                Text = item.Parent.Text,
                Type = item.Parent.Type
            };
            si.Parent = GetParent(item.Parent);

            return si;
        }

        #endregion
    }
}
