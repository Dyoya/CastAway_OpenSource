![header](https://capsule-render.vercel.app/api?type=rect&color=auto&height=300&section=header&text=필%20독&fontSize=60&animation=twinkling&desc=CastAway%20OpenSource%20Project)
<br/>
<img src="https://img.shields.io/badge/Unity-000000?style=flat&logo=Unity&logoColor=white"/> <img src="https://img.shields.io/badge/C%20Sharp-512BD4?style=flat&logo=C%20Sharp&logoColor=white"/>
<br/>
**23-2 오픈소스프로젝트 협업 repository**
- 유니티 버전 : `2021.3.31f1`
<br/><br/>

### 바로가기
- [수정 필요!](https://github.com/Dyoya/CastAway_OpenSource#%EC%88%98%EC%A0%95-%ED%95%84%EC%9A%94)
- [수정 내역](https://github.com/Dyoya/CastAway_OpenSource#%EC%88%98%EC%A0%95-%EB%82%B4%EC%97%AD)
- [참고 자료](https://github.com/Dyoya/CastAway_OpenSource#%EC%B0%B8%EA%B3%A0%EC%9E%90%EB%A3%8C)

<br/><br/>

## 수정 필요!
**아래에 "발견날짜, 내용" 작성**  
**수정 완료시 ~~으로 내용 둘러싸고 [수정 버전] 표시**
> - 230101, 버그버그
> - ~~230102, 버그bug~~ [0.x.1]
- 



<br/><br/>

## 수정 내역
> `[버전]`
> - 아래에 적을 내용은 수정 or 추가 내역
> - 버전은 a.b.c
> - (최종 테스트).(새로운 기능 추가).(버그 수정 or 자잘한 추가)
> - 예시 : `0.9.4`에서 새로운 기능 추가 -> `0.10.0`

### 2023-10-17
`[0.0.0]`
- 유니티 프로젝트 파일 생성

### 2023-11-07
`[0.0.1]`
- 각자 테스트 해볼 수 있는 테스트 씬 추가

### 2023-11-10
`[0.1.0]`
- **MainCamera.cs** 스크립트 추가
    - 플레이어 따라 부드럽게 카메라 이동
    - 플레이어와 카메라 사이에 있는 오브젝트 반투명화
    - Maturial에서 Surface Type - Transparent로 설정하면 투명화가 됨
    - Transparent로 설정했는데 오브젝트가 이상하게 보이면 쉐이더 코드를 수정해야 할 듯함.

### 2023-11-11
`[0.1.1]`
- **Item** 스크립터블 오브젝트 추가
    - Item.cs 스크립트를 통해 수정 가능
    - 인벤토리 및 아이템 상호작용 등에 사용 예정

### 2023-11-12
`[0.2.0]`
- **EnemyFSM.cs** 스크립트 추가
    - 적대 몬스터 스크립트
    - 사용하진 않을 듯 함.
    - 대신 **EnemyBT.cs** 사용
- Scripts 폴더 내 **\Enemy** 폴더 생성
    - **INode.cs** : INode 인터페이스 스크립트
    - **ActionNode.cs** : 노드 스크립트1
    - **SelectorNode.cs** : 노드 스크립트2
    - **SequenceNode.cs** : 노드 스크립트3
    - **BehaviorTreeRunner.cs** : BT 실행 스크립트
    - **EnemyBT.cs** : 적대 몬스터 스크립트



<br/><br/>

## 참고자료
1. [PlayerPref 사용 방법](https://devparklibrary.tistory.com/22)
2. [DontDestroyOnLoad 사용 방법](https://wergia.tistory.com/191)
3. [깃허브로 유니티 프로젝트 관리](https://wergia.tistory.com/238)
4. [메타파일 주의점](https://blog.naver.com/raruz/222852771902)
5. [플레이어와 카메라 사이의 오브젝트 투명화하기](https://daekyoulibrary.tistory.com/entry/Charon-8-플레이어와-카메라-사이의-오브젝트-투명화하기)
