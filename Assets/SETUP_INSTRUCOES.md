# 📋 Instruções de Configuração - Sistema Solar VR

## Pré-requisitos
- Unity 6000.0.29f1
- Google Cardboard XR Plugin instalado
- Asset "The Solar System" importado

---

## Passo 1: Configurar Projeto para Cardboard

### 1.1 Instalar Google Cardboard XR Plugin

1. **Abrir Package Manager**
   - Menu: `Window > Package Manager`
   - Ou pressionar `Ctrl + 9` (Windows) / `Cmd + 9` (Mac)

2. **Adicionar do Git URL**
   - No canto superior esquerdo, clique no dropdown (diz "Unity Registry")
   - Selecione: `+` > `Add package from git URL...`
   - Digite exatamente: `https://github.com/googlevr/cardboard-xr-plugin.git`
   - Clique em `Add`

3. **Aguardar Importação**
   - O Unity baixará e importará o pacote automaticamente
   - Aguarde aparecer na lista de pacotes instalados

### 1.2 Configurar XR Plugin Management

1. **Abrir Project Settings**
   - Menu: `Edit > Project Settings`
   - Ou pressionar `Ctrl + ,` (Windows) / `Cmd + ,` (Mac)

2. **Navegar até XR Plug-in Management**
   - No painel esquerdo, expanda `XR Plug-in Management`
   - Selecione a aba `Android` (se for para Android) ou `iOS` (se for para iOS)

3. **Habilitar Google Cardboard**
   - Na lista de providers, localize `Google Cardboard`
   - **Marque a checkbox** ao lado de `Google Cardboard`
   - Se aparecer algum aviso sobre dependências, aceite/instale

4. **Configurar Cardboard Settings**
   - Ainda em `Project Settings`, procure por `Cardboard` no menu esquerdo
   - Ou vá em: `Project Settings > XR Plug-in Management > Cardboard`
   - Configure os parâmetros de lente (valores padrão geralmente funcionam)

### 1.3 Configurar Build Settings

1. **Abrir Build Settings**
   - Menu: `File > Build Settings`
   - Ou pressionar `Ctrl + Shift + B` (Windows) / `Cmd + Shift + B` (Mac)

2. **Selecionar Plataforma**
   - Selecione `Android` ou `iOS` na lista
   - Clique em `Switch Platform` se necessário (aparece se não estiver na plataforma selecionada)
   - Aguarde o Unity trocar a plataforma

3. **Configurações Adicionais (Android)**
   - Clique em `Player Settings...`
   - Em `Other Settings`:
     - `Minimum API Level`: Android 7.0 (API Level 24) ou superior
     - `Target API Level`: Auto ou mais recente

---

## Passo 2: Configurar Cena Principal

### 2.1 Criar ou Preparar Cena

1. **Criar Nova Cena** (se necessário)
   - Menu: `File > New Scene`
   - Ou: `File > New Scene...` > Selecionar `Basic (Built-in)` > `Create`
   - Salvar: `Ctrl + S` (Windows) / `Cmd + S` (Mac)
   - Nome: `MainVRScene.unity`
   - Salvar em: `Assets/Scenes/`

### 2.2 Configurar Câmera Principal

1. **Selecionar Main Camera**
   - Na Hierarchy, clique em `Main Camera`

2. **Verificar Componentes Existentes**
   - No Inspector, você verá componentes como:
     - `Transform`
     - `Camera`
     - `Audio Listener`
   - Se não tiver `Camera`, algo está errado

3. **Adicionar Script do Cardboard à Câmera**
   
   **MÉTODO 1: Via Component Add**
   - Com a `Main Camera` selecionada no Inspector
   - No Inspector, clique no botão `Add Component`
   - No campo de busca, digite: `Cardboard`
   - Procure por: `CardboardStartup` (ou `Google.XR.Cardboard.Api`)
   - Se aparecer `CardboardStartup`:
     - Clique nele para adicionar
   - Se não aparecer, use o MÉTODO 2

   **MÉTODO 2: Criar GameObject Especial para Cardboard**
   - Na Hierarchy, clique com botão direito > `Create Empty`
   - Nome: `CardboardController`
   - Com `CardboardController` selecionado no Inspector:
     - Clique em `Add Component`
     - Digite: `CardboardStartup`
     - Ou digite: `VrModeController`
     - Adicione o componente encontrado

   **MÉTODO 3: Usar Script da Pasta Samples**
   - Vá em `Assets > Samples > Google Cardboard XR Plugin for Unity > [versão] > Hello Cardboard > Scripts`
   - Arraste o script `CardboardStartup.cs` ou `VrModeController.cs` para a `Main Camera` na Hierarchy
   - Isso adicionará o componente automaticamente

4. **Verificar Configuração da Câmera**
   - `Main Camera` selecionada
   - No Inspector, verifique `Transform`:
     - `Position`: Configure para `(0, 1.6, -10)` (ajuste conforme necessário)
     - `Rotation`: `(0, 0, 0)`
   - Verifique `Camera`:
     - `Clear Flags`: Skybox ou Solid Color
     - `Background`: Cor preta ou azul escuro

5. **Adicionar Script SolarVRGaze à Câmera**
   - Com `Main Camera` selecionada
   - No Inspector: `Add Component`
   - Digite: `SolarVRGaze`
   - Clique no script quando aparecer
   - **NÃO configure ainda** - vamos configurar depois

### 2.3 Configurar Sistema de Gaze/Raycast

1. **Verificar que SolarVRGaze está na Câmera**
   - `Main Camera` deve ter componente `SolarVRGaze` no Inspector

2. **Configurar Parâmetros do SolarVRGaze** (por enquanto deixe padrão)
   - `Gaze Time`: 2.0 (mantenha)
   - `Raycast Distance`: 100 (mantenha)
   - `Interaction Layers`: Everything (mantenha)
   - **Deixe os campos de UI em branco por enquanto** - vamos preencher depois

### 2.4 Criar e Configurar Reticle UI

1. **Criar Canvas Principal**
   - Na Hierarchy: Botão direito > `UI > Canvas`
   - Nome: `Canvas_Reticle`

2. **Configurar Canvas**
   - Com `Canvas_Reticle` selecionado no Inspector:
     - `Render Mode`: **Mude para `World Space`**
     - `Event Camera`: **Arraste `Main Camera` para este campo**
     - `Sort Order`: 100 (deixe padrão)

3. **Ajustar Transform do Canvas**
   - `Canvas_Reticle` selecionado
   - No `Transform`:
     - `Position`: `(0, 0, 5)` (5 unidades à frente da câmera)
     - `Rotation`: `(0, 0, 0)`
     - `Scale`: `(0.01, 0.01, 0.01)` (pequeno para VR)

4. **Criar Reticle Point (Ponto Central)**
   - Com `Canvas_Reticle` selecionado na Hierarchy
   - Botão direito em `Canvas_Reticle` > `UI > Image`
   - Nome: `ReticlePoint`

5. **Configurar ReticlePoint**
   - `ReticlePoint` selecionado no Inspector
   - No componente `Image`:
     - `Source Image`: None (deixe vazio - será um círculo simples)
     - **Dica**: Se quiser um círculo visível, você pode:
       - Criar um sprite circular em `Assets > Create > Sprites > Circle` (se disponível)
       - Ou deixar vazio e usar apenas a cor
     - `Color`: 
       - Clique no quadrado colorido
       - Escolha Branco, ou digite valores RGB: `R: 255, G: 255, B: 255, A: 255`
   - No `Rect Transform`:
     - Clique em `Anchors` e selecione o preset "Center" (centro)
     - Ou configure manualmente:
       - `Min X`: 0.5, `Min Y`: 0.5
       - `Max X`: 0.5, `Max Y`: 0.5
     - `Width`: Digite `20`
     - `Height`: Digite `20`
     - `Pos X`: Digite `0`
     - `Pos Y`: Digite `0`

6. **Criar Reticle Fill (Preenchimento)**
   - Botão direito em `Canvas_Reticle` > `UI > Image`
   - Nome: `ReticleFill`

7. **Configurar ReticleFill**
   - `ReticleFill` selecionado no Inspector
   - No componente `Image`, procure pelo campo `Image Type`:
     - **Se você NÃO vê o campo `Image Type`**: Isso pode acontecer em algumas versões do Unity ou se o componente for `Image` padrão
     - **Solução 1 - Criar Sprite Circular:**
       1. No Project window: Botão direito > `Create > Sprites > Circle` (se disponível)
       2. Se não tiver essa opção, continue com Solução 2
     - **Solução 2 - Usar TextMeshPro Image:**
       1. Delete o componente `Image` atual (botão direito no componente > `Remove Component`)
       2. Adicione: `Add Component` > Digite `TextMeshPro - Image` > Adicione
       3. O `TextMeshPro - Image` tem suporte para Fill
     - **Solução 3 - Criar Material com Shader:**
       1. Mantenha o `Image` componente
       2. Crie um Material: `Assets > Create > Material`
       3. No Material, mude o Shader para um que suporte Fill
     - **Solução 4 - Usar Script Customizado:**
       - O `SolarVRGaze` já gerencia o `fillAmount` programaticamente
       - Você pode usar um `Image` simples e deixar o script controlar via código
   
   - **Se encontrar o campo `Image Type`:**
     - Clique no dropdown ao lado de `Image Type`
     - Selecione: **`Filled`** (não "Simple" ou "Sliced")
     - Após selecionar `Filled`, novos campos aparecerão abaixo:
       - `Fill Method`: Clique no dropdown e selecione `Radial 360`
       - `Fill Origin`: Clique no dropdown e selecione `Top` (ou "Top" ou índice 0)
       - `Fill Amount`: Digite `0` (valor inicial - círculo vazio)
   - **Configurar Cor:**
     - `Color`: Clique no quadrado colorido e escolha Verde ou Ciano
     - Ou digite valores: `R: 0, G: 255, B: 255, A: 255`
   - No `Rect Transform`:
     - Mesmas configurações do ReticlePoint
     - `Width`: 20
     - `Height`: 20
     - `Pos X`: 0
     - `Pos Y`: 0
     - **Importante**: Posição IDÊNTICA ao ReticlePoint (mesmo X e Y exatos)
   
   **Nota Importante**: O `SolarVRGaze` controla o `fillAmount` via código, então mesmo que você não consiga configurar o Fill no Inspector, o reticle ainda funcionará. O importante é que o `ReticleFill` esteja vinculado no componente `SolarVRGaze`.

8. **Vincular Reticle ao SolarVRGaze**
   - Selecione `Main Camera` na Hierarchy
   - No Inspector, no componente `SolarVRGaze`:
     - Arraste `ReticleFill` (da Hierarchy) para o campo `Reticle Fill`
     - Arraste `ReticlePoint` (da Hierarchy) para o campo `Reticle Point`
     - `Normal Color`: Branco (mantenha)

---

## Passo 3: Configurar Sistema Solar e Planetas

### 3.1 Importar Asset do Sistema Solar

1. **Se já importado, pule para próximo passo**

2. **Importar do Asset Store**
   - Abra Asset Store: `Window > Asset Store`
   - Procure: "The Solar System"
   - Ou use o link: https://assetstore.unity.com/packages/3d/environments/sci-fi/the-solar-system-2178
   - Clique em `Import` ou `Download` > `Import`

3. **Se tiver arquivo local**
   - `Assets > Import Package > Custom Package...`
   - Selecione o arquivo `.unitypackage`
   - Clique em `Import`
   - Aguarde importação completa

### 3.2 Adicionar Planetas à Cena

1. **Localizar Prefabs dos Planetas**
   - No Project window, vá em: `Assets > Solar System > Prefabs` (ou caminho similar do asset)
   - Procure pelos prefabs dos planetas

2. **Arrastar Planetas para a Cena**
   - Arraste cada planeta da janela Project para a Hierarchy
   - Ordem sugerida na Hierarchy:
     - `Sun` (ou `Sol`)
     - `Mercury` (ou `Mercurio`)
     - `Venus` (ou `Venus`)
     - `Earth` (ou `Terra`)
     - `Mars` (ou `Marte`)
     - `Jupiter` (ou `Jupiter`)
     - `Saturn` (ou `Saturno`)
     - `Uranus` (ou `Urano`)
     - `Neptune` (ou `Netuno`)

3. **Posicionar Planetas**
   - **Importante**: Os planetas devem estar FIXOS (não orbitando)
   - Para cada planeta, ajuste `Transform > Position`:
     - Exemplo Mercúrio: `(10, 0, 0)`
     - Exemplo Vênus: `(20, 0, 0)`
     - Exemplo Terra: `(30, 0, 0)`
     - E assim por diante (espaçamento sugerido: 10 unidades entre cada)
   - Ajuste conforme o tamanho do seu sistema solar

### 3.3 Configurar Rotação e Luas

1. **Criar GameObject Controlador**
   - Hierarchy: Botão direito > `Create Empty`
   - Nome: `SolarSystemController`

2. **Adicionar Script SolarSystemRealistic**
   - Com `SolarSystemController` selecionado
   - Inspector: `Add Component`
   - Digite: `SolarSystemRealistic`
   - Adicione o script

3. **Configurar Planetas no Script**
   - `SolarSystemController` selecionado
   - No Inspector, no componente `SolarSystemRealistic`:
   - Expanda `Sun`:
     - `Body Transform`: Arraste o GameObject `Sun` da Hierarchy
     - `Rotation Speed`: 1.0 (ajuste conforme necessário)
     - `Rotation Axis`: `(0, 1, 0)` (Y = para cima)
     - `Enable Rotation`: ✅ (marcado)
   - Repita para cada planeta:
     - `Mercury`, `Venus`, `Earth`, `Mars`, `Jupiter`, `Saturn`, `Uranus`, `Neptune`
     - Configure rotação para cada um

4. **Configurar Luas (se houver)**
   - No mesmo script, expanda `Moons`
   - `Size`: Quantidade de luas
   - Para cada lua:
     - `Moon Transform`: Arraste a lua
     - `Planet Transform`: Arraste o planeta pai
     - `Orbit Speed`: Graus por segundo
     - `Orbit Radius`: Distância do planeta

---

## Passo 4: Configurar Sistema de Câmera e Posições

### 4.1 Criar Posição Inicial (Sol)

1. **Criar Empty GameObject**
   - Hierarchy: Botão direito > `Create Empty`
   - Nome: `CameraPos_Sol`

2. **Posicionar em Frente ao Sol**
   - `CameraPos_Sol` selecionado
   - No `Transform`:
     - Se o Sol está em `(0, 0, 0)`:
       - `Position`: `(0, 1.6, -10)` (10 unidades atrás, 1.6 altura)
       - `Rotation`: `(0, 0, 0)` (olhando para frente)
     - Ajuste conforme a posição do seu Sol

3. **Rotacionar para Olhar o Sol**
   - Com `CameraPos_Sol` selecionado
   - Use a ferramenta de rotação (Q no Scene View)
   - Ou ajuste `Rotation` no Inspector para olhar na direção do Sol

### 4.2 Criar Posições de Câmera para Cada Planeta

**Repita este processo para CADA planeta:**

1. **Criar Empty para o Planeta**
   - Hierarchy: Botão direito > `Create Empty`
   - Nome: `CameraPos_[NomePlaneta]`
   - Exemplo: `CameraPos_Mercurio`, `CameraPos_Venus`, etc.

2. **Posicionar em Frente ao Planeta**
   - Selecione o planeta na Hierarchy
   - Anote a `Position` do planeta (ex: `(10, 0, 0)`)
   - Selecione `CameraPos_Mercurio`
   - Configure `Position`:
     - X: mesma do planeta (ex: 10)
     - Y: 1.6 (altura dos olhos)
     - Z: posição do planeta - 5 (ex: se planeta em Z=0, câmera em Z=-5)
   - Ou use `Move Tool` (W) no Scene View para posicionar visualmente

3. **Rotacionar para Olhar o Planeta**
   - Com a posição de câmera selecionada
   - Use `Rotate Tool` (E) ou ajuste `Rotation` no Inspector
   - A câmera deve estar olhando para o planeta

4. **Organizar na Hierarchy (Opcional mas Recomendado)**
   - Arraste cada `CameraPos_[Planeta]` para ser filho do planeta correspondente
   - Facilita organização

### 4.3 Adicionar Script de Movimento à Câmera

1. **Selecionar Main Camera**
   - Na Hierarchy, clique em `Main Camera`

2. **Adicionar CameraSmoothMovement**
   - Inspector: `Add Component`
   - Digite: `CameraSmoothMovement`
   - Adicione o script
   - **Não precisa configurar** - será usado pelo SolarVRManager

---

## Passo 5: Configurar Sistema de Áudio

### 5.1 Criar AudioSource

1. **Criar GameObject para Áudio**
   - Hierarchy: Botão direito > `Create Empty`
   - Nome: `AudioController`

2. **Adicionar AudioSource**
   - Com `AudioController` selecionado
   - Inspector: `Add Component`
   - Digite: `Audio Source`
   - Adicione o componente

3. **Configurar AudioSource**
   - No componente `Audio Source`:
     - `Play On Awake`: ❌ **DESMARQUE** (importante!)
     - `Loop`: ❌ **DESMARQUE**
     - `Volume`: 1.0 (ou ajuste conforme necessário)
     - `Spatial Blend`: 0 (2D - sem espacialização)
     - Deixe os outros campos padrão

### 5.2 Preparar Áudios dos Planetas

1. **Importar ou Criar AudioClips**
   - Coloque seus arquivos de áudio em: `Assets/Audio/` (crie a pasta se não existir)
   - Formatos suportados: `.mp3`, `.wav`, `.ogg`
   - Nomes sugeridos: `Audio_Sol`, `Audio_Mercurio`, `Audio_Venus`, etc.

2. **Importar Áudios**
   - Arraste arquivos de áudio para a pasta `Assets/Audio/`
   - Ou: `Assets > Import New Asset...` > Selecione os arquivos
   - **Importante**: Áudios devem ter no máximo 1 minuto de duração

3. **Verificar AudioClips**
   - Selecione um AudioClip no Project
   - No Inspector, verifique:
     - `Load Type`: Compressed In Memory (padrão)
     - `Compression Format`: Vorbis (padrão)
     - Aguarde Unity processar

---

## Passo 6: Configurar Quizzes para Cada Planeta

### 6.1 Criar Canvas de Quiz para um Planeta (Repita para Cada Planeta)

**Exemplo para Mercúrio:**

1. **Criar Canvas**
   - Hierarchy: Botão direito > `UI > Canvas`
   - Nome: `QuizCanvas_Mercurio`

2. **Configurar Canvas**
   - `QuizCanvas_Mercurio` selecionado no Inspector
   - `Canvas` component:
     - `Render Mode`: **Mude para `World Space`** ⚠️ IMPORTANTE
     - `Event Camera`: **Arraste `Main Camera`** para este campo
   - `Canvas Scaler`: Pode deixar como está ou desabilitar
   - `Graphic Raycaster`: Deixe habilitado

3. **Ajustar Transform do Canvas**
   - `Position`: Posicione ao LADO do planeta (não à frente!)
     - Exemplo: Se Mercúrio em `(10, 0, 0)`
     - Canvas em: `(12, 1.6, 2)` (ao lado direito)
   - `Rotation`: Rotacione para ficar visível da posição da câmera
     - Exemplo: `(0, -45, 0)` (gira para olhar para câmera)
   - `Scale`: `(0.002, 0.002, 0.002)` (pequeno para VR)

### 6.2 Criar Elementos do Quiz dentro do Canvas

**Com `QuizCanvas_Mercurio` selecionado na Hierarchy:**

1. **Criar Painel de Fundo**
   - Botão direito em `QuizCanvas_Mercurio` > `UI > Panel`
   - Nome: `QuizPanel`
   - No `Image` component:
     - `Color`: Preto `(0, 0, 0, 200)` (preto semi-transparente)
   - No `Rect Transform`:
     - `Anchors`: Stretch-Stretch
     - `Left`, `Right`, `Top`, `Bottom`: 0 (ou ajuste para tamanho desejado)

2. **Criar Texto da Pergunta**
   - Botão direito em `QuizPanel` > `UI > Text - TextMeshPro`
   - **Se perguntar sobre TextMeshPro**: Clique em `Import TMP Essentials`
   - Nome: `QuestionText`
   - No componente `TextMeshProUGUI`:
     - `Text`: "Qual é a temperatura média de Mercúrio?"
     - `Font Size`: 48 (ajuste conforme necessário)
     - `Alignment`: Center
     - `Color`: Branco
   - No `Rect Transform`:
     - `Anchors`: Top-Center
     - `Width`: 800
     - `Height`: 100
     - `Pos Y`: -50 (topo do painel)

3. **Criar Primeira Alternativa (Botão A)**
   - Botão direito em `QuizPanel` > `UI > Button - TextMeshPro`
   - Nome: `OptionButton_A`
   - **Configurar Tag**:
     - No topo do Inspector, clique na tag atual (provavelmente "Untagged")
     - Se não existir tag "Selectable", crie:
       - Clique em `Add Tag...`
       - Clique em `+` no painel Tags
       - Digite: `Selectable`
       - Pressione Enter
       - Volte ao GameObject e selecione a tag `Selectable`
   - **Adicionar Collider**:
     - Inspector: `Add Component`
     - Digite: `Box Collider`
     - Adicione
     - No `Box Collider`:
       - `Is Trigger`: ❌ Desmarcado
       - `Size`: Ajuste para cobrir o botão visualmente
   - **Adicionar SelectableObject**:
     - Inspector: `Add Component`
     - Digite: `SelectableObject`
     - Adicione
   - **Configurar Texto do Botão**:
     - Expanda `OptionButton_A` na Hierarchy
     - Selecione `Text (TMP)`
     - No `TextMeshProUGUI`:
       - `Text`: "167°C"
       - `Font Size`: 36
       - `Alignment`: Center
   - **Ajustar Posição**:
     - No `Rect Transform` do botão:
       - `Anchors`: Middle-Left
       - `Width`: 350
       - `Height`: 60
       - `Pos X`: -200
       - `Pos Y`: -100

4. **Criar Segunda Alternativa (Botão B)**
   - Botão direito em `QuizPanel` > `UI > Button - TextMeshPro`
   - Nome: `OptionButton_B`
   - **Repita TODOS os passos do Botão A**:
     - Tag: `Selectable`
     - `Box Collider`
     - `SelectableObject`
     - Texto: "450°C"
     - Posição: `Pos X`: 200 (direita)

5. **Criar Botão Próximo**
   - Botão direito em `QuizPanel` > `UI > Button - TextMeshPro`
   - Nome: `NextButton`
   - Tag: `Selectable`
   - `Box Collider`
   - `SelectableObject`
   - Texto: "Próximo"
   - **Importante**: No Inspector, no componente `Button`:
     - `Interactable`: ❌ **DESMARQUE** (será habilitado quando acertar)
   - No `Rect Transform`:
     - `Pos Y`: -200 (abaixo das alternativas)
   - **Inicialmente Invisível**:
     - No topo do Inspector, desmarque a checkbox ao lado do nome (isso desativa o GameObject)
     - Ou no `Rect Transform`, ajuste `Scale` para `(0, 0, 0)` temporariamente

6. **Criar Texto de Feedback**
   - Botão direito em `QuizPanel` > `UI > Text - TextMeshPro`
   - Nome: `FeedbackText`
   - Texto: "" (vazio)
   - `Font Size`: 40
   - `Color`: Branco
   - No topo do Inspector: **Desmarque checkbox** (inicialmente invisível)
   - Posição: Entre os botões e o botão Próximo

7. **Criar Botão Repetir Áudio** (Opcional mas Recomendado)
   - Botão direito em `QuizPanel` > `UI > Button - TextMeshPro`
   - Nome: `ReplayAudioButton`
   - Tag: `Selectable`
   - `Box Collider`
   - `SelectableObject`
   - Texto: "🔊 Repetir Áudio"
   - Posição: Canto superior direito do painel

### 6.3 Repetir para Todos os Planetas

- Repita os passos 6.1 e 6.2 para cada planeta
- Nomeie os Canvas: `QuizCanvas_Venus`, `QuizCanvas_Terra`, etc.
- Ajuste perguntas e respostas conforme cada planeta

---

## Passo 7: Configurar SolarVRManager

### 7.1 Criar GameObject Principal

1. **Criar Empty GameObject**
   - Hierarchy: Botão direito > `Create Empty`
   - Nome: `SolarVRManager`

2. **Adicionar Script SolarVRManager**
   - Com `SolarVRManager` selecionado
   - Inspector: `Add Component`
   - Digite: `SolarVRManager`
   - Adicione o script

### 7.2 Preencher Referências Gerais

1. **Main Camera**
   - No componente `SolarVRManager`, campo `Main Camera`
   - Arraste `Main Camera` da Hierarchy para este campo

2. **Initial Camera Position**
   - Arraste `CameraPos_Sol` da Hierarchy para este campo

3. **Gaze Controller**
   - Arraste `Main Camera` da Hierarchy para este campo
   - (O `SolarVRGaze` está na Main Camera)

4. **Audio Source**
   - Arraste `AudioController` da Hierarchy para este campo

5. **Reticle Fill**
   - Expanda `Canvas_Reticle` na Hierarchy
   - Arraste `ReticleFill` para este campo

6. **Reticle Point**
   - Arraste `ReticlePoint` para este campo

### 7.3 Configurar Lista de Planetas

**Para CADA planeta na ordem desejada:**

1. **Aumentar Tamanho da Lista**
   - No componente `SolarVRManager`, expanda `Planets`
   - Clique em `+` ou ajuste `Size` para o número de planetas
   - Exemplo: Se tiver 8 planetas, `Size`: 8

2. **Preencher Dados do Primeiro Planeta (exemplo: Mercúrio)**

   - **Planet Name**: Digite `Mercúrio`

   - **Planet Transform**: 
     - Arraste o GameObject do planeta `Mercury` (ou `Mercurio`) da Hierarchy

   - **Camera Position**:
     - Arraste `CameraPos_Mercurio` da Hierarchy

   - **Camera Transition Speed**: `2.0` (ajuste conforme necessário)

   - **Planet Audio**:
     - Arraste `Audio_Mercurio` do Project (pasta Audio)

   - **Audio Trigger Distance**: `50` (não usado no momento, mas pode deixar)

   - **Replay Audio Button**:
     - Expanda `QuizCanvas_Mercurio` > `QuizPanel` na Hierarchy
     - Arraste `ReplayAudioButton` para este campo

   - **Quiz Canvas**:
     - Arraste `QuizCanvas_Mercurio` da Hierarchy

   - **Question Text**:
     - Expanda `QuizCanvas_Mercurio` > `QuizPanel` na Hierarchy
     - Arraste `QuestionText` para este campo

   - **Option Buttons**:
     - `Size`: 2
     - Element 0: Arraste `OptionButton_A`
     - Element 1: Arraste `OptionButton_B`

   - **Next Button**:
     - Arraste `NextButton`

   - **Feedback Text**:
     - Arraste `FeedbackText`

   - **Question**:
     - Digite a pergunta: "Qual é a temperatura média de Mercúrio?"

   - **Option A**:
     - Digite: "167°C"

   - **Option B**:
     - Digite: "450°C"

   - **Correct Answer**:
     - Digite: `1` (se B for correta) ou `0` (se A for correta)

3. **Repetir para Cada Planeta**
   - Configure todos os planetas na ordem desejada

---

## Passo 8: Configurar Tags e Layers

### 8.1 Criar Tag "Selectable"

1. **Abrir Tags and Layers**
   - Menu: `Edit > Project Settings`
   - No menu esquerdo: `Tags and Layers`
   - Ou clique no topo do Inspector em qualquer GameObject e selecione `Add Tag...`

2. **Adicionar Tag**
   - No painel `Tags`, clique em `+`
   - Digite: `Selectable`
   - Pressione Enter
   - Feche o painel

3. **Aplicar Tag nos Botões**
   - Para CADA botão interativo:
     - Selecione o botão na Hierarchy
     - No topo do Inspector, clique na tag atual
     - Selecione `Selectable`

### 8.2 Verificar Layers

1. **No SolarVRGaze**
   - Selecione `Main Camera`
   - No componente `SolarVRGaze`:
     - `Interaction Layers`: Deve estar em `Everything`
     - Ou selecione layers específicos se usar

---

## Passo 9: Testar Botão de Início

### 9.1 Criar Botão de Início (Opcional)

1. **Criar Canvas de Menu**
   - Hierarchy: Botão direito > `UI > Canvas`
   - Nome: `Canvas_Menu`
   - `Render Mode`: `Screen Space - Overlay` (diferente do reticle)

2. **Criar Botão Iniciar**
   - Botão direito em `Canvas_Menu` > `UI > Button - TextMeshPro`
   - Nome: `Button_Start`
   - Texto: "Iniciar Jornada"
   - Posição: Centro da tela

3. **Adicionar StartJourneyButton**
   - Com `Button_Start` selecionado
   - Inspector: `Add Component`
   - Digite: `StartJourneyButton`
   - Adicione
   - No script:
     - `Solar VR Manager`: Arraste `SolarVRManager` da Hierarchy
     - `Start Button`: Arraste `Button_Start`
     - `Start Automatically`: Marque se quiser iniciar automaticamente
     - `Auto Start Delay`: 3 (segundos)

### 9.2 Ou Iniciar Automaticamente

1. **Modificar SolarVRManager**
   - Abra o script `SolarVRManager.cs`
   - No método `Start()`, adicione após a inicialização:
   ```csharp
   // Inicia automaticamente após 3 segundos
   Invoke(nameof(BeginJourney), 3f);
   ```
   - Salve o arquivo

---

## Passo 10: Ajustes Finais e Teste

### 10.1 Verificações Finais

1. **Checklist:**
   - ✅ Google Cardboard habilitado em Project Settings
   - ✅ Main Camera tem `SolarVRGaze` e Cardboard configurado
   - ✅ Reticle UI criado e vinculado
   - ✅ Planetas adicionados à cena e posicionados
   - ✅ Posições de câmera criadas para cada planeta
   - ✅ AudioSource configurado
   - ✅ Quizzes criados para cada planeta
   - ✅ Todos os botões têm tag `Selectable` e `Box Collider`
   - ✅ SolarVRManager configurado com todos os planetas
   - ✅ Tags "Selectable" criadas

2. **Testar no Editor (Preview)**
   - Pressione `Play` no Unity
   - Verifique no Game View se tudo aparece
   - Verifique console para erros

3. **Testar no Dispositivo**
   - `File > Build Settings`
   - Clique em `Build And Run`
   - Teste com Cardboard físico

### 10.2 Ajustes de Posicionamento

- **Se quiz não está visível:**
  - Ajuste `Position` e `Rotation` do Canvas de quiz
  - Teste valores diferentes até ficar visível

- **Se reticle não aparece:**
  - Verifique escala do Canvas_Reticle
  - Verifique posição (deve estar à frente da câmera)
  - Verifique se está vinculado ao SolarVRGaze

- **Se câmera não se move:**
  - Verifique se `Camera Position` está preenchido no SolarVRManager
  - Verifique se `Main Camera` está vinculada

---

## 🎯 Estrutura Final Esperada da Hierarchy

```
MainVRScene
├── Main Camera
│   ├── SolarVRGaze (componente)
│   └── CameraSmoothMovement (componente)
├── CardboardController (opcional - se usou método 2)
├── AudioController
│   └── Audio Source (componente)
├── SolarSystemController
│   └── SolarSystemRealistic (componente)
├── SolarVRManager
│   └── SolarVRManager (componente)
├── Canvas_Reticle
│   ├── ReticlePoint
│   └── ReticleFill
├── Canvas_Menu (opcional)
│   └── Button_Start
├── CameraPos_Sol
├── Sun
│   └── CameraPos_Sun (se organizou como filho)
├── Mercury
│   ├── CameraPos_Mercurio
│   └── QuizCanvas_Mercurio
│       └── QuizPanel
│           ├── QuestionText
│           ├── OptionButton_A
│           ├── OptionButton_B
│           ├── NextButton
│           ├── FeedbackText
│           └── ReplayAudioButton
├── Venus
│   ├── CameraPos_Venus
│   └── QuizCanvas_Venus
│       └── ... (mesma estrutura)
└── ... (outros planetas)
```

---

## ✅ Checklist de Configuração

Marque conforme completar:

- [ ] Google Cardboard instalado e habilitado
- [ ] Main Camera configurada com Cardboard
- [ ] SolarVRGaze adicionado à Main Camera
- [ ] Reticle UI criado e vinculado
- [ ] Planetas importados e posicionados
- [ ] Posições de câmera criadas
- [ ] AudioSource configurado
- [ ] Áudios importados
- [ ] Quiz criado para cada planeta
- [ ] Todos os botões têm tag Selectable
- [ ] Todos os botões têm Box Collider
- [ ] SolarVRManager configurado completamente
- [ ] Testado no editor
- [ ] Testado no dispositivo

---

**Agora seu projeto está completamente configurado!** 🎉

Teste e ajuste conforme necessário. Se encontrar problemas, verifique o console do Unity para mensagens de erro.

