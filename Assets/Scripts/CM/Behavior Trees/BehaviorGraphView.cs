#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace CM.Behavior_Trees
{
    public class BehaviorGraphView : GraphView
    {
        public BehaviorGraphView(BehaviorEditorWindow editorWindow)
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Add a button to create a new node
            var nodeCreateButton = new Button(() => { AddElement(CreateNode("New Node")); });
            nodeCreateButton.text = "Create Node";
            Add(nodeCreateButton);
        }

        // Method to create a new node
        private Node CreateNode(string nodeName)
        {
            var node = new Node
            {
                title = nodeName,
            };
            
            var inputPort = GeneratePort(node, Direction.Input);
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            var button = new Button(() => { RemoveElement(node); });
            button.text = "X";
            node.titleButtonContainer.Add(button);

            node.RefreshExpandedState();
            node.RefreshPorts();

            return node;
        }

        // Method to generate a port
        private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
    }
}
#endif