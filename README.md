![header](https://capsule-render.vercel.app/api?type=rect&color=auto&height=300&section=header&text=필%20독&fontSize=60&animation=twinkling&desc=CastAway%20OpenSource%20Project)
<br/>
<img src="https://img.shields.io/badge/Unity-000000?style=flat&logo=Unity&logoColor=white"/> <img src="https://img.shields.io/badge/C%20Sharp-512BD4?style=flat&logo=C%20Sharp&logoColor=white"/> 
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-4-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->
<br/>

**23-2 오픈소스프로젝트 협업 repository**
- 유니티 버전 : `2021.3.31f1`

<br/>

### Contributors
<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/Dyoya/"><img src="https://avatars.githubusercontent.com/u/118094189?v=4" width="100px;" alt="Dyoya"/><br /><sub><b>Dyoya (HyeongMin Kim)</b></sub></a><br /> <a href="https://github.com/Dyoya/CastAway_OpenSource/commits?author=Dyoya" title="Commit">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/CHTuna/"><img src="https://avatars.githubusercontent.com/u/45841115?v=4" width="100px;" alt="CHTuna"/><br /><sub><b>CHTuna (DongWon Kang)</b></sub></a><br /> <a href="https://github.com/Dyoya/CastAway_OpenSource/commits?author=CHTuna" title="Commit">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/KDHen/"><img src="https://avatars.githubusercontent.com/u/97778379?v=4" width="100px;" alt="KDHen"/><br /><sub><b>KDHen (DongHyeon Kang)</b></sub></a><br /> <a href="https://github.com/Dyoya/CastAway_OpenSource/commits?author=KDHen" title="Commit">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/Hyung-Junn/"><img src="https://avatars.githubusercontent.com/u/102523742?v=4" width="100px;" alt="Hyung-Junn"/><br /><sub><b>Hyung-Junn (HyeongJun Kim)</b></sub></a><br /> <a href="https://github.com/Dyoya/CastAway_OpenSource/commits?author=Hyung-Junn" title="Commit">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

### 바로가기
- [Scripts 디렉터리](https://github.com/Dyoya/CastAway_OpenSource/tree/main/Assets/Scripts)
- [수정 필요!](https://github.com/Dyoya/CastAway_OpenSource#%EC%88%98%EC%A0%95-%ED%95%84%EC%9A%94)
- [수정 내역](https://github.com/Dyoya/CastAway_OpenSource#%EC%88%98%EC%A0%95-%EB%82%B4%EC%97%AD)
- [참고 자료](https://github.com/Dyoya/CastAway_OpenSource#%EC%B0%B8%EA%B3%A0%EC%9E%90%EB%A3%8C)

<br/><br/>

## 수정 필요!
**아래에 "발견날짜, [버전] 내용" 작성**  
**수정 완료시 아래에 [수정 버전] 표시**
> - 230101, [0.x.1] 버그버그
- 231110, 플레이어가 밟고 있는 오브젝트까지 투명화되고 있음
    - [0.2.1] 수정 완료
- 231130, Bear Boss의 점프 패턴에서 착지 위치에 플레이어가 있을 시, 플레이어가 땅 속으로 뚫고 떨어짐



<br/><br/>

## 수정 내역
> `[버전]`
> - 아래에 적을 내용은 수정 or 추가 내역
> - 버전은 a.b.c
> - ~~(최종 테스트).(새로운 기능 추가).(버그 수정 or 자잘한 추가)~~
> - ~~예시 : `0.9.4`에서 새로운 기능 추가 -> `0.10.0`~~

### 2023-10-17
`[0.0.0]`
- 유니티 프로젝트 파일 생성

### 2023-11-07
`[0.0.1]`
- 각자 테스트 해볼 수 있는 테스트 씬 추가

### 2023-11-10
`[0.1.0]`
- **MainCamera.cs**, **TransparentObject.cs** 스크립트 추가
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
    - 웬만하면 FSM 대신 **EnemyBT.cs** 사용 예정
- Scripts 폴더 내 **\Enemy** 폴더 생성
    - **INode.cs** : INode 인터페이스 스크립트
    - **ActionNode.cs** : 노드 스크립트1
    - **SelectorNode.cs** : 노드 스크립트2
    - **SequenceNode.cs** : 노드 스크립트3
    - **BehaviorTreeRunner.cs** : BT 실행 스크립트
    - **EnemyBT.cs** : 적대 몬스터 스크립트

### 2023-11-14
`[0.2.1]`
- **TransparentObject.cs** 버그 수정
    - 플레이어가 밟고 있는 오브젝트까지 투명화되는 현상 수정  

`[0.2.2]`
- **EnemyBT.cs** 구조 수정
    - Die 관련 노드 세분화
    - 불필요 폴더 1개 정리  

`[0.2.3]`
- 사람, 토끼 에셋 추가
- **EnemyBT.cs** 수정
    - Return 노드 구조 수정
    - Idle 노드 추가

`[0.2.4]`
- Scene/CutScene/PlaneCrashCutScene : 비행기 추락 컷씬 추가
- Scene/CutScene/BossCutScene : 보스 몬스터(곰) 출현 컷씬 추가
- Test_DW : 탈출 컷씬 테스트 중 (수정 필요)

`[0.2.5]`
- TitleMenu : 시작 메뉴(기능 일부 구현 추가 구현 필요)
- Test_DW : ESC 시 Pause 메뉴 구현 (기능 일부 구현 추가 구현 필요)

`[0.2.6]`
- Player : 플레이어 ProgressBar구현(아직 체력이 0이 되었을 때 죽는것, 에너지가 0이 되었을 때 달리지 못하도록 하는 것은 추가 예정)
- Player : 플레이어 아이템 먹기 및 소비 UI 구현(버리기 기능 추가 구현 필요)

### 2023-11-16
`[0.2.7]`
- **EnemyBT.cs** 수정
    - 공격 쿨타임 추가
    - 애니메이션 부자연스러움 수정

`[0.2.8]`
- Player : 미니맵, 맵 추가(플레이어 위치 표현)

### 2023-11-17
`[0.2.9]`
- **EnemyBT.cs** 수정  
      - 순찰 기능 추가  
      - NevMeshAgent 어색하게 회전하는 현상 수정  

### 2023-11-18
`[0.3.0]`
- EscMenuEvent, MenuEvent => UIEvent로 통합

### 2023-11-19
`[0.3.1]`
- Player : 아이템 줍기, 죽었을 때, 마우스 왼쪽 클릭(공격) 애니메이션 추가(아이템 주울 때 아이템 위로 올라가게 되면 플레이어 난동 애니메이션 오류있음 난중에 수정 예정)

`[0.3.2]`
- Player : 아이템 광클해도 안먹어지는 현상 수정함
- PlaneCrash 컷씬에서 처음에 간략하게 스토리 내용을 추가 할 수 있게 수정

`[0.3.3]`
- Player : 아이템 간혹 2개 동시에 먹어지는 현상 수정함 이제 isTrigger에 있는 아이템 중에 제일 가까운 것만 먹음
- save and load : 플레이어 위치 저장과 로드 스크립트 작성 -> 플레이어가 들고 있는 아이템까지 저장해야됨

### 2023-11-23
`[0.3.4]`
- **EnemyBT.cs** 수정  
    - Return 삭제  
    - Navmesh Component 오픈소스 사용  
- **EscapeEnemyBT.cs** 스크립트 추가  
    - 토끼와 같은 도망가는 동물을 위한 스크립트

`[0.3.5]`
- Player : FishingZone, 각 아이템을 획득하고나서의 UI, 각 아이템들을 먹을 때 UI 변경
- Item : 낚시대, 낚시줄, 완전한 낚시대 생성

### 2023-11-25
`[0.3.6]`
- **BearBossBT.cs** 스크립트 추가  
    - 보스 Behavior Tree  
    - 전체적인 틀 제작  
    - 세부 패턴 코드 필요  
- **BearBossSkill.cs** 스크립트 추가  
    - 보스 스킬 클래스  

`[0.3.7]`
- **CookController** 스크립트 추가  
    - 요리 범위 판정 스크립트
- **CookInteract** 스크립트 추가  
    - 요리 상호작용 스크립트
    - 테스트용 (조정필요)
- **Note** 스크립트 추가  
    - 노트 이동 스크립트
- **NoteController** 스크립트 추가  
    - 노트 조작 스크립트
- 인벤토리 1번 2번 아이템에 따른 상호작용 추가 필요

`[0.3.8]`
- **BearBossBT.cs** 수정  
    - 돌진 패턴 추가  
- **BearRushTrigger.cs** 스크립트 추가  
    - 돌진 판정 스크립트

### 2023-11-26
`[0.3.9]`
- **BearBossBT.cs** 수정
    - 돌진 시전 시, 돌진 범위 보여줌
    - 내려찍기 패턴 추가
    - 내려찍기 시전 시, 피격 범위 보여줌
- **BearStampTrigger.cs** 스크립트 추가
    - 내려찍기 판정 스크립트

`[0.4.0]`
- player : 아이템 버리기 추가, 아이템 소비아이템만 섭취 가능, 곡갱이, 도끼 아이템 추가 -> 조합 아이템 생성 수정 필요
- environment : 돌맹이(부술때랑 깨졌을 때 애니메이션 추가)

### 2023-11-30
`[0.4.1]`
- **BearBossBT.cs** 수정
    - 점프 패턴 추가
    - 점프 패턴에서 몇몇 오류 발견, 수정 예정
    - 내려찍기 패턴의 Range 활성화 관련 코루틴 수정

### 2023-12-01
`[0.4.2]`
- **BearBossBT.cs** 수정
    - 점프 패턴 전후 포효 추가
    - 내려찍기 패턴 땅 오브젝트 자연스럽게 사라지도록 수정

### 2023-12-02
`[0.4.3]`
- **CookController.cs** 수정 
    - 요리 미니게임 성공에 따른 음식 생성 
- **slot.cs** 수정
    - 요리 인벤토리 슬롯에 요리표시를 위한 함수 추가
- **item.cs** 수정
    - 요리와 관련된 bool 특성 추가
 
`[0.4.4]`
- 낚시 애니메이션 추가, 낚시 UI추가

`[0.4.5]`
- **BearBossBT.cs** 수정
    - 간단한 사운드 추가
    - 점프 착지 패턴의 콜라이더 수정
  
### 2023-12-03
`[0.4.6]`
- **BearBossBT.cs** 수정
    - 일반 공격 및 패턴 공격 성공 시 플레이어의 체력 감소
    - 패턴 내부 쿨타임 제대로 작동하지 않던 현상 수정
- **Fire.cs** 추가
    - 모닥불 관리 스크립트
    - 일정 시간 후, 불 꺼짐
    - 불이 켜져있을 때, 플레이어가 불 위에 있는 경우 플레이어의 체력 감소
    - 상호작용 추가 필요(나뭇가지 소모 - 불 유지시간 증가)

### 2023-12-04
`[0.4.7]`
- BossCutScene 수정
    - 보스 등장씬 수정

### 2023-12-05
`[0.4.8]`
- 요리 키 변경 (E -> C)
- 모닥불 미니게임 추가
- 모닥불 관련 스크립트 추가
    - **FireGame.cs** : 불 피우기 게임 스크립트
    - **FireSlider.cs** : 불 슬라이더 관련 스크립트
    - **FireTextTrigger.cs** : 모닥불 상호작용 스크립트

`[0.4.9]`
- 낚시 키 변경 (O,L -> 위 아래 키), 속도 변경 게이지 늦게 차고 물고기 속도 올림
- 바위 나무 캐는 범위 확장, 돌맹이, 나뭇가지 아이템 추가, 생성
- 빌더 텝
  - CraftManuals.cs : 조건 확인을 통한 빌더 여부 추가
  - InventoryUI.cs : 빌더와 관련된 플레이어의 아이템을 확인 및 소모 추가

### 2023-12-06
`[0.5.0]`
- **FireOnTrigger.cs** 스크립트 추가
    - 모닥불 유지시간 증가 기능

`[0.5.1]`
- **MapObjectData.cs** 스크립트 추가
    - 맵 세이브 관련 스크립트
- **ObtainableObject** 스크립트 추가
    - 나무, 암석 등 맵 세이브 관련 스크립트
- **SaveAndLoad** 스크립트 수정
    - 인벤토리, 아이템 이동, 플레이어 위치, 플레이어 상태 등 저장 및 로드 기능

### 2023-12-07
`[0.5.2]`
- **SaveAndLoad** 스크립트 수정
    - 세이브 파일 로드시 무한루프 상태가되는 현상 수정
 
`[0.5.3]`
- 도끼, 곡괭이 보유 상태에서 다른 아이템을 버렸을 때 비활성화 되는 버그 수정

### 2023-12-08
`[0.5.4]`
- **EnemyBT.cs**, **BearBossBT.cs**, **EscapeEnemyBT.cs** 수정
    - 피격 데미지 관련 코드 수정
    - 사망 관련 코드 수정
- **PlayerAttackTrigger.cs** 수정
    - 플레이어 공격 코드 수정

### 2023-12-09
`[0.5.5]`
- Enemy 스포너 추가
- **EnemyManger.cs** 스크립트 추가
    - Enemy 스포너 관리 스크립
- 모닥불 코드 수정
- **EndingTrigger.cs** 스크립트 추가
    - 엔딩 조건 판별 기능
    - 구조를 위한 돌, 모닥불 설치 기능
 
### 2023-12-10
`[0.5.6]`
- CraftManual.cs : 건축키 마우스 포인터 수정, 낚시대 제작 추가
- EnemyBT.cs, BearBossBT.cs : 적 사냥시 아이템 생성 
- 아이템 관련 이미지 배경 제거
- 탈출존에서 아이템 소비, 모닥불 나뭇가지 아이템 소비 작성필요!!

`[0.5.7]`
- Boss 동굴 씬 추가
- Bear, Rabbit 스포너 배치
- 모닥불-요리 연결

`[0.5.8]`
- 나뭇가지, 돌 아이템 소비추가
- 도끼, 곡괭이, 낚시대 아이템 생성 완성
- 건축 하는동안 공격 애니메이션 없앰
- 프리뷰 건축 마우스 커서 없앰
- 보스, 적 고기 아이템 프리펩 넣어야됨
- EndTrigger.cs : inventory 객체 참조 시켜야됨
- 아이템 value 조정해여됨 요리된 아이템, 아이템, 잘 요리된 아이템

<br/><br/>

## 참고자료
1. [PlayerPref 사용 방법](https://devparklibrary.tistory.com/22)
2. [DontDestroyOnLoad 사용 방법](https://wergia.tistory.com/191)
3. [깃허브로 유니티 프로젝트 관리](https://wergia.tistory.com/238)
4. [메타파일 주의점](https://blog.naver.com/raruz/222852771902)
5. [플레이어와 카메라 사이의 오브젝트 투명화하기](https://daekyoulibrary.tistory.com/entry/Charon-8-플레이어와-카메라-사이의-오브젝트-투명화하기)
6. [NavMesh Component1](https://algorfati.tistory.com/25)
7. [NavMesh Component2](https://twosouls.tistory.com/7)
8. [모닥불](https://ansohxxn.github.io/unity%20lesson%203/ch8-1/)
9. [건축 기능](https://ansohxxn.github.io/unity%20lesson%203/ch9-1/)
10. [세이브&로드](https://ansohxxn.github.io/unity%20lesson%203/ch12-8/)
