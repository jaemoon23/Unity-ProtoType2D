# Unity-ProtoType2D

Unity 2D 프로토타입 프로젝트

## Git Workflow

### Branch Protection Rules

main 브랜치는 보호되어 있습니다.

| 규칙 | 설명 |
|------|------|
| PR 필수 | main에 직접 push 불가, PR을 통해서만 머지 |
| 1명 승인 필요 | 다른 팀원 1명의 승인이 있어야 머지 가능 |
| 자기 승인 불가 | 본인이 올린 PR은 본인이 승인할 수 없음 |
| 대화 해결 필수 | 모든 리뷰 코멘트가 resolved 되어야 머지 가능 |
| Force Push 차단 | main 브랜치에 force push 불가 |
| 삭제 차단 | main 브랜치 삭제 불가 |

### Branch Naming Convention

브랜치 이름은 다음 규칙을 따라야 합니다.

```
<type>/<description>
```

#### 허용되는 타입

| 타입 | 용도 | 예시 |
|------|------|------|
| `feature/` | 새 기능 | `feature/login-system` |
| `fix/` | 버그 수정 | `fix/null-reference-error` |
| `hotfix/` | 긴급 수정 | `hotfix/crash-on-start` |
| `refactor/` | 리팩토링 | `refactor/player-controller` |
| `docs/` | 문서 | `docs/readme-update` |
| `test/` | 테스트 | `test/unit-tests` |

#### 규칙

- description은 **소문자, 숫자, 하이픈(-)** 만 사용
- GitHub Actions에서 자동으로 브랜치 이름 검사

#### 예시

```bash
# 새 기능 개발
git checkout -b feature/inventory-system

# 버그 수정
git checkout -b fix/player-death-bug

# 긴급 수정
git checkout -b hotfix/crash-on-start
```

### Merge Method

PR 머지 시 **Squash and Merge** 방식을 사용합니다.

- 여러 커밋이 하나로 합쳐져서 깔끔한 히스토리 유지

## Discord Webhook

GitHub 이벤트가 Discord 채널에 알림됩니다.

- Issues (이슈 생성, 닫힘, 수정)
- Issue comments (이슈 댓글)
- Pull requests (PR 생성, 머지, 닫힘)
