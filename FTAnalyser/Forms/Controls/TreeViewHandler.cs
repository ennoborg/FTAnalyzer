using System.Linq;
using System.Windows.Forms;

namespace FTAnalyzer.Forms.Controls
{
    public class TreeViewHandler
    {
        static TreeViewHandler instance;
        TreeNode mainformTreeRootNode;
        TreeNode placesTreeRootNode;

        TreeViewHandler() => ResetData();

        public static TreeViewHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TreeViewHandler();
                }
                return instance;
            }
        }

        public void ResetData()
        {
            mainformTreeRootNode = null;
            placesTreeRootNode = null;
        }

        #region Location Tree Building
        TreeNode[] BuildTreeNodeArray(bool mainForm)
        {
            TreeNodeCollection nodes;
            if (mainForm)
                nodes = mainformTreeRootNode.Nodes;
            else
                nodes = placesTreeRootNode.Nodes;
            TreeNode[] result = new TreeNode[nodes.Count];
            nodes.CopyTo(result, 0);
            return result;
        }

        public void RefreshTreeNodeIcon(FactLocation location)
        {
            if (location is null) return;
            string[] parts = location.GetParts();
            TreeNode currentM = mainformTreeRootNode;
            TreeNode currentP = placesTreeRootNode;
            foreach (string part in parts)
            {
                if (part.Length == 0 && !Properties.GeneralSettings.Default.AllowEmptyLocations) break;
                if (mainformTreeRootNode != null && currentM != null)
                {
                    TreeNode childM = currentM.Nodes.Find(part, false).FirstOrDefault();
                    currentM = childM;
                }
                if (placesTreeRootNode != null && currentP != null)
                {
                    TreeNode childP = currentP.Nodes.Find(part, false).FirstOrDefault();
                    currentP = childP;
                }
            }
        }
        #endregion
    }
}
