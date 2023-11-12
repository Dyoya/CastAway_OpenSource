/*
    행동 트리(Behavior Tree) 실행할 때 사용
 */
public class BehaviorTreeRunner
{
    // 루트 노드
    INode _rootNode;

    public BehaviorTreeRunner(INode rootNode)
    {
        _rootNode = rootNode;
    }

    // 행동 트리 실행
    public void Operate()
    {
        // 루트 노드에서 부터 각 노드의 Evaluate 함수 호출 -> 전체 트리 실행
        _rootNode.Evaluate();
    }
}