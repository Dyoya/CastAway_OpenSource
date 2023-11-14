## 바로가기
- [메인 디렉터리](https://github.com/Dyoya/CastAway_OpenSource)
- [Scripts 디렉터리](https://github.com/Dyoya/CastAway_OpenSource/tree/main/Assets/Scripts)

## 노드 관련
### INode.cs
- 노드 인터페이스 스크립트

### ActionNode.cs
- 행동을 정의한 노드
- 리프 노드로 사용

### SelectorNode.cs
- 여러 행동 중 하나만 실행해야할 때 사용하는 노드
- 성공한 노드가 있으면 해당 노드 실행 후 종료

### SequenceNode.cs
- 여러 행동을 순서대로 진행해야 할 때 사용하는 노드
- 자식 노드 중에 실패한 노드가 있을 때 까지 진행

## BT
### BehaviorTreeRunner.cs
- 행동 트리를 실행할 때 사용하는 스크립트

### EnemyBT.cs
- 행동 트리(Behavior Tree)로 만든 Enemy 스크립트

## FSM
### EnemyFSM.cs
- 유한 상태 머신(Finite State Machine)으로 만든 Enemy 스크립트
- 이번 프로젝트에서는 FSM이 아닌 BT 사용할 예정