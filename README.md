本リポジトリは、ポートフォリオ掲載作品のソースコード共有用リポジトリです。
作品の全容・デモ動画・概要につきましては、[ポートフォリオ](https://mkaoy-portfolio.vercel.app/works/B4_zemi_soturon.html)をご覧ください。


## 開発環境
- Unity 2021.3.4f1
- 動作環境: Windows

### OpenAI API キーの設定
- 本システムを実行するには、別途 **OpenAI API キー** が必要です。
- 実行すると、画面上に **OpenAI API キーの入力フィールド** が表示されますので、ご自身の API キーを入力して開始してください。

※セキュリティ保護のため、本システム内に API キーは保持・保存されません。実行時に都度ご入力ください。

## 主な実装スクリプト
実装の中心となる主要なスクリプトです。全て `Assets/Scripts/` 内にあります。

- CreateNewChara.cs
- RadarChart.cs
- UILineRenderer.cs
- UIManager_Button.cs
- GenerateSceneSituation.cs
- SetAPIKey.cs
- ChatGPTClient.cs
- GPTManager.cs
- GPTResponseParser.cs
- CharaDatabase.cs
- CharaEditor.cs
- UpdateCharaData.cs
- CharaStatusTextManager.cs
- GenerateCharaDescription.cs
- UIManager_SceneSb.cs
- RandomGenerateSettings.cs
- GenerateChats.cs
- GeneratePersonality.cs
- CreateIF_Personality.cs
- CreatePatternUI.cs
- CSVSavingManager.cs
- AbleGenerateChats.cs
- EndManager.cs
- InputSpaceRemover.cs

※ `Assets/Scripts/` 内には試作段階のコンポーネントや検証用スクリプトも含まれていますが、主要な動作は上記のスクリプトで制御されています。