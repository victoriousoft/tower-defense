# Git návod


## Initial setup

Naklonování repozitáře
```
$ git clone <url>
```

Pak jde otevřít v Unity

## Fíčury
Pro každou novou fíčuru [udělejte issue](https://github.com/victoriousoft/tower-defense/issues/new)

## Branche
[WA tutoriál](https://kf-ga.github.io/0_introduction/04_git.html#vytvareni-vetvi-v-git)

Žádnou práci pls nědelat na main branch, na každou fíčuru bude branch

2) Před vytvořením se přehodím na main a pullnu nejnovější změny
```
$ git checkout main && git pull
```

1) Vyvtoření branche
```
$ git branch <nazev-branche>
```

3) Změna na branch
```
$ git checkout <nazev-branche>
```

Když je hotovo, stačí na githubu [otevřít pull request](https://github.com/victoriousoft/tower-defense/compare).

V tom pull requestu bude na prvním řádku

`Fixes #<cislo-issue>` (pak se totiž ten issue automaticky zavře s mergem)

Tracking: https://github.com/orgs/victoriousoft/projects/4/views/1
