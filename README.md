# SolutionInspector ![Icon](https://raw.githubusercontent.com/chrischu/SolutionInspector/master/media/icon.png)
[![Develop Build][appveyor_develop]](https://ci.appveyor.com/project/chrischu/solutioninspector)
[![Master Build][appveyor_master]](https://ci.appveyor.com/project/chrischu/solutioninspector)
[![Coverage][coverage]](http://chrischu.github.io/SolutionInspector/CoverageReports/coverageReport.html)
[![GitHub Release Version][github_release]](https://github.com/chrischu/SolutionInspector/releases)
[![NuGet Release Version (SolutionChecker.exe)][nuget_release_exe]](https://www.nuget.org/packages/SolutionInspector.exe/)
[![NuGet Release Version (SolutionChecker.Api)][nuget_release_api]](https://www.nuget.org/packages/SolutionInspector.Api/)
[![NuGet Release Version (SolutionChecker.DefaultRules)][nuget_release_rules]](https://www.nuget.org/packages/SolutionInspector.DefaultRules/)
[![License][license]](https://raw.githubusercontent.com/chrischu/SolutionInspector/master/LICENSE)

Inspects VisualStudio solutions against violations against a plethora of different rules to keep your solution tidy.

## Documentation
For current change log/work in progress see [Changelog](CHANGELOG.md).
For future plans see [Roadmap](ROADMAP.md).

[appveyor_develop]: https://img.shields.io/appveyor/ci/chrischu/SolutionInspector/develop.svg?label=develop&style=flat&logo=data%3aimage%2fpng%3bbase64%2ciVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAk6QAAJOkBUCTn%2bAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAabSURBVHic7ZtriFVVFMf%2fW8f3O1%2bZpj210gx7mIb5qqTsQ1GKFSpRiWQQUVSU2psYLAKTvkhI3%2frQGzMchFSSUtDSTEtNU1TyrTO%2bBh3n14d973i8d59zzzn73DtC%2fj8N5%2b691n%2btOXvvtddaR7qE%2fzdMJZUBRlJ%2fST0ldZXUSVKDpJOSaiXtNcbsqySnsjoA6CNpvKRxkm6XNFBSuxLT6iRtlbRa0nJJK40xh8vJM1MAXYAZwE9AI%2f5oAJYCU4EOzW1fKIBewFvAsQyMDkMtMB%2b4vLntbQLQHngfOF1GwwtxHJgNtGlu4ycCOytoeCG2AOOaw%2fAq7Ot%2brhmNz6MRuyxaVcr4nsAvzWuzEz8C3ZLak%2bgYBAZIqpE0KKmiCmGzpPuNMbvjTojtgJzxqyT1S0GsktgpaZQxZm%2bcwS3iDAJ6SFqqi994SbpKUg1wWZzBJR0AtJa0WNINfrwqisGSvgGqSg2M8wZUSxrhTanyGC3pnVKDIvcAYKKk70uNu4jRKLspLgsbEGoY0E7SJklXl4FYJbFd0hBjTL3rx6g1MlflM%2f6opD8kHZR0SFI3ST0kDZXUPWNd10p6TdKbsWcAfcg%2btt8FzAGGAs69BzDAYGAusDdD3SeBXkkc8EGGyncDU4CWsQlYDq2AZ7E3wCxQHVdxF%2bxtKwvMBzomMdzBpx82t%2bCLOqBzHIUzMlB2HJjiY3gBp%2fbAsgx4PRVHma%2b3twCDY%2bgZCDwITAbuwabPosa3A9Z4cltRilQf%2fNJY3xLxmmFf52pgf8j8NcDThG%2bSA7EbWlo0Ar2jHPBESsENwOvYrK9LrgFmEX9vWQP0D5H1UkqOeTwW5YBPUwg8BEyIkGmABSnkbgeudMjrABxJIS%2bPhVEOWJ9Q2FrsNTlK5mwPsitwLAdgnofMX8OItiDZ%2bloEtC1h%2fCCg3oMswDSH3BEe8k7gWqrAgJgC6oGZUYYHZC70IJrHaofc1sApD5lFS0vA8BgTdwN3xjS%2bDTb4yAI9HfJXe8i7Iy8nuL46lbBpuaTbjDFrHGRudIwfEkNmXNzieHbUQ14Tr6ADwkJWJH0oaYIx5kDRjzBV0vOOeVkmTl2OPJGFvKADGh0Dj0uaYox52RjTUPgj8ICkRbIV3kLEyjfGhOsuH7kBl0CTrcF8QKFHt0h6xBiz2SUBGCnpS0mtJLmivyMeBAux0fHsGg95x%2fN%2fVLkeytbrhxtj6lyzgZtkU2Xtc49csYD7vE2ObcaYPQX6W8pmf9Oiya7gaxrMo4eWobEhao2kYNr5ZgrO1lyjw88eJPP4xPFstM47Pw3cNQMuTD4Mc%2fzeA%2fgz5GgZ7hg%2f2eOoIqerqKGCdKF1HhecHoUb1dbA3xccbdjExhKF1wcmFT4wxnwh6buQ8aVwWtI0Y8xpBw%2bfXEPQxiIHBKOusQGlrSV9JanovxzAM7izP08q%2bVI4I2mSMWat47cXZXuM0qIosmwC8GjgVdmBvcm1AD6P%2bXo5M6%2fYG9wi4uUa%2fgachRigN%2f7R5cNRDuiOvdvnMYZk660ee0KEyR8LfA2ccczdCDwHODc37D9jcQqDgzhLqRI6UBOYkCY1vZUShUmgLTAMuBfrlJKvNDDT03iAH0rpEbYbyxfryLCRCZsKO5EBr8fjKPPNuOTxD4Fbl4fxVfgnQ8FmroqWV1G8bow5KWmBL3HZSG0VtoPM51Y4V9GnT1x8bIw5FWskdjPMqjgCcBj4CBhJSIUIR5YmN%2f5sBvprCdn8oqrDsyW9F8tjyXBCNhg5IHv%2f6Cp7mXrBGNN0RmNjivWyxU1fvGqMmZdoBjbtFBb2Zomj2Jtlof40GWoXNpG2hQ4YR3l7AQ8Ctzr0PpSR%2fAbg7lTGB8i8mxGZQuwHhjr09QL2ZaRjjpfxOUItgeUZEcpjD1CUMsNGe0sy0rGUkBJbGid0AX7LiNgu4LoQPbMy0rEB6JqJ8QFyfbHBjQ%2b2EV7zG4Rf4TOPrUQVQD2d0Ifk5bM8%2fgL6hsjNKtr7HbiiLMYHyHYDViYktoGIHh2y2WiXEqcDJCMnJGmXX4dttQ2TdRcXXsGToiHHJVEPUiYA7sMmMMKwg4irMdAJWwZPi03AqEra7DKiLfA24RvYv8Bn2GTHGOB6crcybJYoDWqBV6jURxJxgA1gqomfskrT%2fnYIeIMUH0ZUDEBnYDq2qyuLz%2bbO5WRNx7PlzoVyfzjZW%2fajyfE6%2f%2bFkqW%2f%2fjqn4w0mfSnAkKt4Fjm1O6CXbH9xR0jnZK3KdpN2uCvQlXEL58B9i0MZjNagBygAAAABJRU5ErkJggg%3d%3d
[appveyor_master]: https://img.shields.io/appveyor/ci/chrischu/SolutionInspector/master.svg?label=master&style=flat&logo=data%3aimage%2fpng%3bbase64%2ciVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAk6QAAJOkBUCTn%2bAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAabSURBVHic7ZtriFVVFMf%2fW8f3O1%2bZpj210gx7mIb5qqTsQ1GKFSpRiWQQUVSU2psYLAKTvkhI3%2frQGzMchFSSUtDSTEtNU1TyrTO%2bBh3n14d973i8d59zzzn73DtC%2fj8N5%2b691n%2btOXvvtddaR7qE%2fzdMJZUBRlJ%2fST0ldZXUSVKDpJOSaiXtNcbsqySnsjoA6CNpvKRxkm6XNFBSuxLT6iRtlbRa0nJJK40xh8vJM1MAXYAZwE9AI%2f5oAJYCU4EOzW1fKIBewFvAsQyMDkMtMB%2b4vLntbQLQHngfOF1GwwtxHJgNtGlu4ycCOytoeCG2AOOaw%2fAq7Ot%2brhmNz6MRuyxaVcr4nsAvzWuzEz8C3ZLak%2bgYBAZIqpE0KKmiCmGzpPuNMbvjTojtgJzxqyT1S0GsktgpaZQxZm%2bcwS3iDAJ6SFqqi994SbpKUg1wWZzBJR0AtJa0WNINfrwqisGSvgGqSg2M8wZUSxrhTanyGC3pnVKDIvcAYKKk70uNu4jRKLspLgsbEGoY0E7SJklXl4FYJbFd0hBjTL3rx6g1MlflM%2f6opD8kHZR0SFI3ST0kDZXUPWNd10p6TdKbsWcAfcg%2btt8FzAGGAs69BzDAYGAusDdD3SeBXkkc8EGGyncDU4CWsQlYDq2AZ7E3wCxQHVdxF%2bxtKwvMBzomMdzBpx82t%2bCLOqBzHIUzMlB2HJjiY3gBp%2fbAsgx4PRVHma%2b3twCDY%2bgZCDwITAbuwabPosa3A9Z4cltRilQf%2fNJY3xLxmmFf52pgf8j8NcDThG%2bSA7EbWlo0Ar2jHPBESsENwOvYrK9LrgFmEX9vWQP0D5H1UkqOeTwW5YBPUwg8BEyIkGmABSnkbgeudMjrABxJIS%2bPhVEOWJ9Q2FrsNTlK5mwPsitwLAdgnofMX8OItiDZ%2bloEtC1h%2fCCg3oMswDSH3BEe8k7gWqrAgJgC6oGZUYYHZC70IJrHaofc1sApD5lFS0vA8BgTdwN3xjS%2bDTb4yAI9HfJXe8i7Iy8nuL46lbBpuaTbjDFrHGRudIwfEkNmXNzieHbUQ14Tr6ADwkJWJH0oaYIx5kDRjzBV0vOOeVkmTl2OPJGFvKADGh0Dj0uaYox52RjTUPgj8ICkRbIV3kLEyjfGhOsuH7kBl0CTrcF8QKFHt0h6xBiz2SUBGCnpS0mtJLmivyMeBAux0fHsGg95x%2fN%2fVLkeytbrhxtj6lyzgZtkU2Xtc49csYD7vE2ObcaYPQX6W8pmf9Oiya7gaxrMo4eWobEhao2kYNr5ZgrO1lyjw88eJPP4xPFstM47Pw3cNQMuTD4Mc%2fzeA%2fgz5GgZ7hg%2f2eOoIqerqKGCdKF1HhecHoUb1dbA3xccbdjExhKF1wcmFT4wxnwh6buQ8aVwWtI0Y8xpBw%2bfXEPQxiIHBKOusQGlrSV9JanovxzAM7izP08q%2bVI4I2mSMWat47cXZXuM0qIosmwC8GjgVdmBvcm1AD6P%2bXo5M6%2fYG9wi4uUa%2fgachRigN%2f7R5cNRDuiOvdvnMYZk660ee0KEyR8LfA2ccczdCDwHODc37D9jcQqDgzhLqRI6UBOYkCY1vZUShUmgLTAMuBfrlJKvNDDT03iAH0rpEbYbyxfryLCRCZsKO5EBr8fjKPPNuOTxD4Fbl4fxVfgnQ8FmroqWV1G8bow5KWmBL3HZSG0VtoPM51Y4V9GnT1x8bIw5FWskdjPMqjgCcBj4CBhJSIUIR5YmN%2f5sBvprCdn8oqrDsyW9F8tjyXBCNhg5IHv%2f6Cp7mXrBGNN0RmNjivWyxU1fvGqMmZdoBjbtFBb2Zomj2Jtlof40GWoXNpG2hQ4YR3l7AQ8Ctzr0PpSR%2fAbg7lTGB8i8mxGZQuwHhjr09QL2ZaRjjpfxOUItgeUZEcpjD1CUMsNGe0sy0rGUkBJbGid0AX7LiNgu4LoQPbMy0rEB6JqJ8QFyfbHBjQ%2b2EV7zG4Rf4TOPrUQVQD2d0Ifk5bM8%2fgL6hsjNKtr7HbiiLMYHyHYDViYktoGIHh2y2WiXEqcDJCMnJGmXX4dttQ2TdRcXXsGToiHHJVEPUiYA7sMmMMKwg4irMdAJWwZPi03AqEra7DKiLfA24RvYv8Bn2GTHGOB6crcybJYoDWqBV6jURxJxgA1gqomfskrT%2fnYIeIMUH0ZUDEBnYDq2qyuLz%2bbO5WRNx7PlzoVyfzjZW%2fajyfE6%2f%2bFkqW%2f%2fjqn4w0mfSnAkKt4Fjm1O6CXbH9xR0jnZK3KdpN2uCvQlXEL58B9i0MZjNagBygAAAABJRU5ErkJggg%3d%3d
[coverage]: http://chrischu.github.io/SolutionInspector/CoverageReports/coverageBadge.svg
[github_release]: https://img.shields.io/github/release/chrischu/SolutionInspector.svg?label=Release&style=flat&logo=data%3aimage%2fpng%3bbase64%2ciVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAk6QAAJOkBUCTn%2bAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAWtSURBVHic7Zt5iFVVHMe%2fx6dmmeuEGgm2EVrWhJhWtFBMpk5K%2fWFZtJcFQVAG9Y%2bZtFD%2fBf1pf9g6TCQaamlaKQWVhJOhLVRq1mQu4YjamM3y6Y9z33R93nvfOe8uM9B84MFbfue3neWe7Un99PO%2fxhRlCBglaYqkcyWNljRC0sjg50PBq03SDkktxphDRfiVWwKAYZLmBq%2bpsoH7sFPSV5JWSVpljDmarYc5AcwGlgPtZEc78C4wq7fjiwVoADZnGHQcW4F5QGHdNxGgvqDAK9kM1Pdm4AOBp4DjvRB8mQ7gJWBw0cGfA7T0YuCVbAHOLir4y4C9vRxwFH8AU33jGeAZ%2fE2SNkoa62uoAMZJ2gjM9inkPJIC10laJ6nY%2fuZPh6RGY8wGF2GnBACTJX2m%2f2ZufZ3Dkq4xxnxTTbBqAoDxkr6UdFYGjhVJq6Tpxpg9SUKJCcBONtZLakgQ2yRpraR6STdKqvNy05%2ffA59%2bkp1mX54gu17STGMMNVkCHnYYfRtC8iXgNuxjKWvWAbOAASF7cxzKPZQUY2wLACZI2iZpWEL5I5LqjDEdEeUbJL0ouxAqc1TS95IOyq782oLvRwWvOkkTJZ0eKrNF0kJjzKcRNoYEuk5N8PGwpEuMMbsTZE4GaHLI7pYqOkrAg8A9wEVAycFuKZC9F1hQrQywzcHPJt%2fgJwFdDopXeinOAWC1g5%2bdwKSo8nEToacTfgtzSs2eZ4eLDyVJi5y0Aec51j5U6QJFgF0eu9AJnLQpE1XLd8R8H0U9kPdjLxbgDEkXO4qXJN1e%2bWVUoPM8fOiQHb17i5GS%2fvGQvzXxV2CiY3Mqc38q9zMA%2b5Tx4cIkZY97KPq4wDgTATZ5%2bP1YuGxlF5jiYXdJas%2bzY4mHbHyMwHeOWdyV2uWMAXY7%2br49XC48rz5N0gWO9j7M0vmMWO8oNzGIVdKJXeBM2UeFC9urixSOq08lhXa0wgnw2ew44CFbFPs8ZHtiDSdghIeCYx6yReHjU2QChteioA%2fhU4E9suEEdHso6IsJGO0h21V%2bE06ATxPqi%2fuDPj61l9%2bEE3DQQ8F0D9mimOYh2xNrz5YYMFbSXkcFf0kaaYzp9DCaG8BA2QsWQx2LjDHGHJBObAH7ZQNzYaikmc4e5s8MuQd%2fRNKf5Q89CQi2jrd6GF3gIZs3iTu%2fFXwd3iavXAz57PA0Apd6yOcC9o5Ao0eRlvCHygT4LHFLkpbisNObF4HtVyUN9CgWHyMwDP8LD68ROqwoCuz2%2bRuevv4NJI8VwEpPpQDvAIVNjoA6YEUNfi53UX5LDYoBWoH55NgasNdy7gT21OjjHBcjg4BfIgo3YS9EzQDeBrpjjPwALMSeKmcV%2bATgSeDnGgMH2IHreAU8GqFgD6ENReAGoK2K0R%2bxiVsMOG%2b3AdOAJUBz4HgWPOKT8SFEt4J24L6Q3NXYm1rV2IXHGAGMAX7LKHCAndiDVHew%2fTmKLkI3NrG1W427vYxbvQsyCh7A56yjxwGDPZOPYhfBvhp2YNqQYPw4oT04D%2fsjsMdZafnAO%2fiQE%2bOJ7%2beLQ3KDgVc4eQ5xDFiawv72lMG3kXYwxj4Wo0b8%2fVRMKoBxQCMwF7gC8NllirL9SYrgu4GbUwUfcuSFGCPPZ2Ig3q7L2X8cz2XpiAHejDG0CLsez5wUCXidrG%2bTYydI78UY%2fBZ7cfpa7AHrcOx09XzA9fg6ymYtCVhBThVSXoAs83RoTQp7vgl4CxjkY8Nr3m6M6ZL0gKRnJbnevStipdgt6RlJd0XdWMsF7B29fQ61sjaFDZcWsBd7ibsmaq4dY8xqSZMlNSu5NeTVApDUJGmyMabmbpaNJ3AV8EVMDX2UQm9cC%2fgcuDLLGDIBuB54nxOnsM0p9C0L6ekE1mCv7WdGLv%2b8wk4%2f58ue1rxsjPk1hZ4nZC9INxtjWrPzsp9%2b%2bpH0L66JSj1j%2ffqIAAAAAElFTkSuQmCC
[nuget_release_exe]: https://img.shields.io/nuget/v/SolutionInspector.Exe.svg?label=Exe&style=flat&logo=data%3aimage%2fpng%3bbase64%2ciVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAk6QAAJOkBUCTn%2bAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAASnSURBVHic7ZtPqBVVHMc%2fx16WSoaaWJYZhBmVUUEg9EdCs0VaEiW0qEVtgqgWLVrlomjVKrIgahFFiQRRGhoUvhe4KKIWmas0%2f1Wi8sL3Mqinvk%2bLue%2fyspl5c8%2bdc%2beafuDC3Lnn9zu%2f8z1%2f5szM78J5zm1C0Q%2fq1cAy4DTwXQjhWM%2biahJ1sbrNf3NKfV%2bd23R8SWk1%2frDF%2fKhe2nScycjp%2bTxebzrOJKiL1PEKAoyqFzUdb11Mm3S8jJJFcRKXANckiaYBJgtwugO7U3UH0jjqPPVkhSlwTL2g6Xjroj0CQgjDwKYKNhtDCJ2MlrMHdY66q6T3v1CnNx1nUtTZ6mvqyKSGH1U3%2fB8bX7YVvhC4FjgJ7I8Z9uocYCWwHFgKzAPqFHEUOATsAobItuzW6D8O9QZ1s%2fpXhUW1TvaozzU2UtUF6jtm9w5N8rO6vteNv1nd33DDz%2bQts6mcvPFr1BMNN7aIL9WZRbFX2fpO1fhbgJ3ArG59JeRj4OG8BbIrAdQFwLfAoryfgW3AFmAP2aZrCbAOWN1NvZFsCCG8XKtHswUvjz3q7SV2d6gHejMD2oypS%2bps%2fI3mr%2fZ71fkV7BeqB3sogOpHdQqwOaeCcUt6PsfHip42P4vvpjoaP8f8Tc5nEb529FQC3TC5%2fmlFgU3BSiDvqdCWCF8xNt3w4OQvsQIsLzi%2fN8LXT5ExxHKr2T0KEC%2fA0oLzMZfVrvciEfVdN%2fElVoCi9wMxl5kiMVNy2cRBrAAzCs6vi%2fAVY9Mt7fhjBShitXpn1cLqfUDl8jXSnnZ1CwDwobpwqkLqYuDdBPV3RAoBFgHfqCuKCrR6%2fmvg8gT1d8RAIr9XAUPqIPAp2eVxYvVdRzPDPpdUAkxwT%2bvTt6QWoC7GgMHW5zfgKNml7ArgbmAVxVem%2blG%2f79G%2bfVR90Sleyauz1OfV4Yp%2bHzkbBPjK7IFLJ3HNtdor%2frYAKa4CdfABcG8I4UgnRiGE34G1wJtVbfpRgJ3AkyGEsRjj1gucZ4CtVcr3mwCjZA8v%2f%2b7GSQhhHHiMbLEspd8EeLXTYV9ECGEEeGmqcv0kwBhQd%2f7R28DxsgL9JMCOVq%2fVRmsd2V5Wpp8EGErkd7Dsx34S4Jcm%2fPaTAMOJ%2fJam%2bMYKkCJHKFUa7rycc%2b0st1gBTkTalXFlAp9Ffv%2bYOIgV4FCkXRl3JfAJkPdg5sDEQawAP0TalbFKrfUVuzoA3H%2fG6RFg38SXWAGGIu3KmAE8VbPPx%2fnvGrA9hNBdpqsazF6B182wNf0nQZ2pHsqp49E6%2fKM%2bm0AAze7nu0rFNeugTTm%2bD1vXNFOnm%2bUCpOANNWp6thr%2fSoHfeqeYuj6RAKpb1dkdxjPT%2fJ5X3W22KNaLujGhCEfUp50i8VEdUJ8wf86rHlevz7OtI0tsAPicLGcgFcfJ7uoGgV%2bBI8B8YCHZU%2bE15O%2f4INu1rg0hlN4VdoV6sdm%2fyvqNUfWBZA0%2fQ4Rg9gh7rNk2t9ltwbBPLcRi9T2r%2fQErBcfUF6yYMJ0sO8MsG%2bshspyc21LV02KEbB36BNgaQvizqmFP0lPMcnKWkt3y1rXfHydbHA8C%2b7re3p7nHOUfus2s8WdkXxcAAAAASUVORK5CYII%3d
[nuget_release_api]: https://img.shields.io/nuget/v/SolutionInspector.Api.svg?label=API&style=flat&logo=data%3aimage%2fpng%3bbase64%2ciVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAk6QAAJOkBUCTn%2bAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAASnSURBVHic7ZtPqBVVHMc%2fx16WSoaaWJYZhBmVUUEg9EdCs0VaEiW0qEVtgqgWLVrlomjVKrIgahFFiQRRGhoUvhe4KKIWmas0%2f1Wi8sL3Mqinvk%2bLue%2fyspl5c8%2bdc%2beafuDC3Lnn9zu%2f8z1%2f5szM78J5zm1C0Q%2fq1cAy4DTwXQjhWM%2biahJ1sbrNf3NKfV%2bd23R8SWk1%2frDF%2fKhe2nScycjp%2bTxebzrOJKiL1PEKAoyqFzUdb11Mm3S8jJJFcRKXANckiaYBJgtwugO7U3UH0jjqPPVkhSlwTL2g6Xjroj0CQgjDwKYKNhtDCJ2MlrMHdY66q6T3v1CnNx1nUtTZ6mvqyKSGH1U3%2fB8bX7YVvhC4FjgJ7I8Z9uocYCWwHFgKzAPqFHEUOATsAobItuzW6D8O9QZ1s%2fpXhUW1TvaozzU2UtUF6jtm9w5N8rO6vteNv1nd33DDz%2bQts6mcvPFr1BMNN7aIL9WZRbFX2fpO1fhbgJ3ArG59JeRj4OG8BbIrAdQFwLfAoryfgW3AFmAP2aZrCbAOWN1NvZFsCCG8XKtHswUvjz3q7SV2d6gHejMD2oypS%2bps%2fI3mr%2fZ71fkV7BeqB3sogOpHdQqwOaeCcUt6PsfHip42P4vvpjoaP8f8Tc5nEb529FQC3TC5%2fmlFgU3BSiDvqdCWCF8xNt3w4OQvsQIsLzi%2fN8LXT5ExxHKr2T0KEC%2fA0oLzMZfVrvciEfVdN%2fElVoCi9wMxl5kiMVNy2cRBrAAzCs6vi%2fAVY9Mt7fhjBShitXpn1cLqfUDl8jXSnnZ1CwDwobpwqkLqYuDdBPV3RAoBFgHfqCuKCrR6%2fmvg8gT1d8RAIr9XAUPqIPAp2eVxYvVdRzPDPpdUAkxwT%2bvTt6QWoC7GgMHW5zfgKNml7ArgbmAVxVem%2blG%2f79G%2bfVR90Sleyauz1OfV4Yp%2bHzkbBPjK7IFLJ3HNtdor%2frYAKa4CdfABcG8I4UgnRiGE34G1wJtVbfpRgJ3AkyGEsRjj1gucZ4CtVcr3mwCjZA8v%2f%2b7GSQhhHHiMbLEspd8EeLXTYV9ECGEEeGmqcv0kwBhQd%2f7R28DxsgL9JMCOVq%2fVRmsd2V5Wpp8EGErkd7Dsx34S4Jcm%2fPaTAMOJ%2fJam%2bMYKkCJHKFUa7rycc%2b0st1gBTkTalXFlAp9Ffv%2bYOIgV4FCkXRl3JfAJkPdg5sDEQawAP0TalbFKrfUVuzoA3H%2fG6RFg38SXWAGGIu3KmAE8VbPPx%2fnvGrA9hNBdpqsazF6B182wNf0nQZ2pHsqp49E6%2fKM%2bm0AAze7nu0rFNeugTTm%2bD1vXNFOnm%2bUCpOANNWp6thr%2fSoHfeqeYuj6RAKpb1dkdxjPT%2fJ5X3W22KNaLujGhCEfUp50i8VEdUJ8wf86rHlevz7OtI0tsAPicLGcgFcfJ7uoGgV%2bBI8B8YCHZU%2bE15O%2f4INu1rg0hlN4VdoV6sdm%2fyvqNUfWBZA0%2fQ4Rg9gh7rNk2t9ltwbBPLcRi9T2r%2fQErBcfUF6yYMJ0sO8MsG%2bshspyc21LV02KEbB36BNgaQvizqmFP0lPMcnKWkt3y1rXfHydbHA8C%2b7re3p7nHOUfus2s8WdkXxcAAAAASUVORK5CYII%3d
[nuget_release_rules]: https://img.shields.io/nuget/v/SolutionInspector.DefaultRules.svg?label=Rules&style=flat&logo=data%3aimage%2fpng%3bbase64%2ciVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAk6QAAJOkBUCTn%2bAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAASnSURBVHic7ZtPqBVVHMc%2fx16WSoaaWJYZhBmVUUEg9EdCs0VaEiW0qEVtgqgWLVrlomjVKrIgahFFiQRRGhoUvhe4KKIWmas0%2f1Wi8sL3Mqinvk%2bLue%2fyspl5c8%2bdc%2beafuDC3Lnn9zu%2f8z1%2f5szM78J5zm1C0Q%2fq1cAy4DTwXQjhWM%2biahJ1sbrNf3NKfV%2bd23R8SWk1%2frDF%2fKhe2nScycjp%2bTxebzrOJKiL1PEKAoyqFzUdb11Mm3S8jJJFcRKXANckiaYBJgtwugO7U3UH0jjqPPVkhSlwTL2g6Xjroj0CQgjDwKYKNhtDCJ2MlrMHdY66q6T3v1CnNx1nUtTZ6mvqyKSGH1U3%2fB8bX7YVvhC4FjgJ7I8Z9uocYCWwHFgKzAPqFHEUOATsAobItuzW6D8O9QZ1s%2fpXhUW1TvaozzU2UtUF6jtm9w5N8rO6vteNv1nd33DDz%2bQts6mcvPFr1BMNN7aIL9WZRbFX2fpO1fhbgJ3ArG59JeRj4OG8BbIrAdQFwLfAoryfgW3AFmAP2aZrCbAOWN1NvZFsCCG8XKtHswUvjz3q7SV2d6gHejMD2oypS%2bps%2fI3mr%2fZ71fkV7BeqB3sogOpHdQqwOaeCcUt6PsfHip42P4vvpjoaP8f8Tc5nEb529FQC3TC5%2fmlFgU3BSiDvqdCWCF8xNt3w4OQvsQIsLzi%2fN8LXT5ExxHKr2T0KEC%2fA0oLzMZfVrvciEfVdN%2fElVoCi9wMxl5kiMVNy2cRBrAAzCs6vi%2fAVY9Mt7fhjBShitXpn1cLqfUDl8jXSnnZ1CwDwobpwqkLqYuDdBPV3RAoBFgHfqCuKCrR6%2fmvg8gT1d8RAIr9XAUPqIPAp2eVxYvVdRzPDPpdUAkxwT%2bvTt6QWoC7GgMHW5zfgKNml7ArgbmAVxVem%2blG%2f79G%2bfVR90Sleyauz1OfV4Yp%2bHzkbBPjK7IFLJ3HNtdor%2frYAKa4CdfABcG8I4UgnRiGE34G1wJtVbfpRgJ3AkyGEsRjj1gucZ4CtVcr3mwCjZA8v%2f%2b7GSQhhHHiMbLEspd8EeLXTYV9ECGEEeGmqcv0kwBhQd%2f7R28DxsgL9JMCOVq%2fVRmsd2V5Wpp8EGErkd7Dsx34S4Jcm%2fPaTAMOJ%2fJam%2bMYKkCJHKFUa7rycc%2b0st1gBTkTalXFlAp9Ffv%2bYOIgV4FCkXRl3JfAJkPdg5sDEQawAP0TalbFKrfUVuzoA3H%2fG6RFg38SXWAGGIu3KmAE8VbPPx%2fnvGrA9hNBdpqsazF6B182wNf0nQZ2pHsqp49E6%2fKM%2bm0AAze7nu0rFNeugTTm%2bD1vXNFOnm%2bUCpOANNWp6thr%2fSoHfeqeYuj6RAKpb1dkdxjPT%2fJ5X3W22KNaLujGhCEfUp50i8VEdUJ8wf86rHlevz7OtI0tsAPicLGcgFcfJ7uoGgV%2bBI8B8YCHZU%2bE15O%2f4INu1rg0hlN4VdoV6sdm%2fyvqNUfWBZA0%2fQ4Rg9gh7rNk2t9ltwbBPLcRi9T2r%2fQErBcfUF6yYMJ0sO8MsG%2bshspyc21LV02KEbB36BNgaQvizqmFP0lPMcnKWkt3y1rXfHydbHA8C%2b7re3p7nHOUfus2s8WdkXxcAAAAASUVORK5CYII%3d
[license]: https://img.shields.io/github/license/chrischu/SolutionInspector.svg?style=flat