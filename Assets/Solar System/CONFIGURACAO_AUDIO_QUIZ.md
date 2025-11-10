# Configuração do Sistema de Áudio no Quiz

Este documento descreve os passos para configurar o sistema de áudio no componente de quiz do projeto de realidade virtual.

## Visão Geral

O sistema foi modificado para:
1. Tocar automaticamente o áudio do planeta quando a câmera chegar perto
2. Exibir o quiz apenas após o áudio terminar
3. Permitir repetir o áudio através de um botão (sem ocultar o quiz)

## Pré-requisitos

- Os arquivos de áudio devem estar na pasta `Assets/Resources/`
- Os arquivos de áudio devem ter os seguintes nomes:
  - `mercurio.mp3`
  - `venus.mp3`
  - `terra.mp3`
  - `marte.mp3`
  - `jupter.mp3`
  - `saturno.mp3`
  - `netuno.mp3`
  - `Sol.mp3`

## Passo a Passo de Configuração

### 1. Verificar os Arquivos de Áudio

1. Abra o Unity Editor
2. Navegue até a pasta `Assets/Resources/`
3. Verifique se todos os arquivos de áudio estão presentes:
   - `mercurio.mp3`
   - `venus.mp3`
   - `terra.mp3`
   - `marte.mp3`
   - `jupter.mp3`
   - `saturno.mp3`
   - `netuno.mp3`
   - `Sol.mp3`

### 2. Configurar o Componente PlanetQuiz

Para cada planeta na cena:

1. Selecione o GameObject que contém o componente `PlanetQuiz`
2. No Inspector, localize o componente `PlanetQuiz`

#### ⚠️ IMPORTANTE: Busca Automática de Componentes

O script `PlanetQuiz` agora busca automaticamente os componentes no prefab `PlanetQuizCanvas` se eles não forem atribuídos manualmente. Isso facilita a configuração!

**Componentes que são buscados automaticamente:**
- **Quiz Canvas**: Busca automaticamente um Canvas filho ou no objeto pai
- **Feedback Text**: Busca automaticamente um objeto chamado "FeedBackText" (ou que contenha "feedback" no nome)
- **Next Planet Button**: Busca automaticamente um botão chamado "NextPlanetButton"
- **Question Panels**: Busca automaticamente painéis que contenham botões de opção

**Como funciona:**
- Se você não atribuir os componentes manualmente, o script tentará encontrá-los automaticamente
- Verifique o Console do Unity para ver quais componentes foram encontrados
- Se algum componente não for encontrado, você verá um aviso no console

#### Configuração Manual (Opcional)

Se preferir configurar manualmente ou se a busca automática não funcionar:

1. **Abra o Prefab PlanetQuizCanvas**:
   - Na pasta `Assets/Solar System/Prefabs/`, encontre o prefab `PlanetQuizCanvas`
   - Clique duas vezes para abrir no modo de edição de prefab
   - OU expanda o prefab na Hierarchy da cena (clique na seta ao lado do nome)

2. **Encontre os Componentes no Prefab**:
   - **Quiz Canvas**: O GameObject raiz do prefab (chamado "PlanetQuizCanvas")
   - **FeedBackText**: Um GameObject filho chamado "FeedBackText" (componente TMP_Text)
   - **NextPlanetButton**: Um GameObject filho chamado "NextPlanetButton" (componente Button)
   - **Question Panels**: Painéis que contêm os botões "Button_Option1" e "Button_Option2"

3. **Atribua os Componentes**:
   - **Quiz Canvas**: Arraste o Canvas do quiz para este campo
   - **Question Panels**: Configure os painéis de perguntas (array)
     - Encontre o painel que contém os botões de opção
     - Arraste esse painel para o array "Question Panels"
   - **Feedback Text**: Arraste o componente TMP_Text "FeedBackText"
   - **Next Planet Button**: Arraste o botão "NextPlanetButton"

   #### Campos de Áudio (Novos):
   - **Audio Source**: 
     - Se já existir um AudioSource no GameObject, ele será usado automaticamente
     - Caso contrário, um AudioSource será criado automaticamente
     - Você pode criar manualmente um AudioSource e arrastá-lo para este campo
   
   #### Campos Opcionais:
   - **Replay Audio Button**: Arraste o botão "Tocar Áudio Novamente" (opcional, mas recomendado)
     - Este botão permite ao usuário repetir o áudio após o quiz aparecer
     - O botão não oculta o quiz quando clicado
     - **Nota**: Se não criar este botão, o usuário não poderá repetir o áudio

### 3. Configurar o Botão de Repetir Áudio (Opcional mas Recomendado)

**Opção 1: Adicionar ao Prefab PlanetQuizCanvas (Recomendado)**
1. Abra o prefab `PlanetQuizCanvas` no modo de edição
2. Clique com o botão direito no Canvas > **UI > Button - TextMeshPro**
3. Renomeie o botão para "ReplayAudioButton" (ou qualquer nome que contenha "replay" ou "audio")
4. Posicione o botão onde desejar (recomendado: próximo às perguntas)
5. Altere o texto do botão para "Tocar Áudio Novamente" ou "Repetir Áudio" (evite emojis como 🔊 que podem não estar na fonte)
6. Salve o prefab
7. O script `PlanetQuiz` buscará automaticamente este botão se:
   - O nome do botão contiver "replay" ou "audio"
   - OU o texto do botão contiver "replay", "tocar" ou "audio"

**Opção 2: Adicionar na Cena**
1. No Canvas do quiz na cena, crie um novo botão
2. Posicione o botão onde desejar (recomendado: próximo às perguntas)
3. Adicione um texto ao botão (ex: "🔊 Tocar Áudio Novamente")
4. No componente `PlanetQuiz`, arraste este botão para o campo **Replay Audio Button**

### 4. Verificar o XRRigMover

1. Selecione o GameObject que contém o componente `XRRigMover`
2. Verifique se os `targets` estão configurados corretamente
3. Cada `target` deve ter um `targetReference` que aponta para o Transform do planeta
4. O `PlanetQuiz` deve estar como filho (ou filho de filho) do `targetReference`

### 5. Configurar PlanetIdentifier (Opcional)

Para uma identificação mais precisa do planeta:

1. Adicione o componente `PlanetIdentifier` ao GameObject do planeta
2. Configure o campo **Planet Index** com o índice correto:
   - 0 = Mercúrio
   - 1 = Vênus
   - 2 = Terra
   - 3 = Marte
   - 4 = Júpiter
   - 5 = Saturno
   - 6 = Netuno
   - 7 = Sol

**Nota**: Se o `PlanetIdentifier` não estiver presente, o sistema tentará identificar o planeta através do `XRRigMover` ou pelo nome do GameObject.

### 6. Configurar o AudioSource (Recomendado)

Para melhor controle do áudio:

1. Selecione o GameObject que contém o `PlanetQuiz`
2. Adicione um componente `AudioSource` (se não existir)
3. Configure o AudioSource:
   - **Play On Awake**: Desmarcado (não deve tocar automaticamente)
   - **Loop**: Desmarcado (o áudio não deve repetir automaticamente)
   - **Volume**: Ajuste conforme necessário (recomendado: 0.7 - 1.0)
   - **Spatial Blend**: 0 (2D) ou ajuste conforme necessário para VR

### 7. Testar a Configuração

1. Execute a cena no Unity Editor
2. Verifique se:
   - Quando a câmera chega perto de um planeta, o áudio começa a tocar automaticamente
   - O quiz não aparece enquanto o áudio está tocando
   - Após o áudio terminar, o quiz aparece quando o usuário olha para o planeta (gaze)
   - O botão de repetir áudio funciona corretamente (se configurado)
   - O botão de repetir áudio não oculta o quiz

## Solução de Problemas

### O áudio não está tocando

1. Verifique se o arquivo de áudio existe em `Assets/Resources/`
2. Verifique se o nome do arquivo está correto (sem extensão .mp3 no código)
3. Verifique se o `PlanetIdentifier` está configurado corretamente (ou o nome do GameObject contém o nome do planeta)
4. Verifique se o `AudioSource` está configurado corretamente
5. Verifique os logs do Unity para mensagens de erro

### O quiz aparece antes do áudio terminar

1. Verifique se o método `ActivateQuiz()` está sendo chamado apenas após o gaze completo
2. Verifique se a corrotina `WaitForAudioToFinish()` está funcionando corretamente
3. Verifique os logs do Unity para ver o fluxo de execução

### O áudio não é carregado

1. Verifique se o arquivo está na pasta `Resources/` (com R maiúsculo)
2. Verifique se o nome do arquivo corresponde exatamente ao mapeamento no código
3. Verifique se o arquivo foi importado corretamente no Unity (verifique o Inspector do arquivo)
4. Verifique os logs do Unity para mensagens de aviso sobre áudio não encontrado

### O botão de repetir áudio não funciona

1. Verifique se o botão foi atribuído ao campo **Replay Audio Button** no `PlanetQuiz`
2. Verifique se o botão tem um componente `Button` configurado
3. Verifique se o `AudioSource` está configurado corretamente

## Estrutura de Arquivos Esperada

```
Assets/
├── Resources/
│   ├── mercurio.mp3
│   ├── venus.mp3
│   ├── terra.mp3
│   ├── marte.mp3
│   ├── jupter.mp3
│   ├── saturno.mp3
│   ├── netuno.mp3
│   └── Sol.mp3
└── Solar System/
    └── Scripts/
        ├── PlanetQuiz.cs
        ├── XRRigMover.cs
        └── PlanetIdentifier.cs
```

## Notas Importantes

1. **Ordem dos Planetas**: A ordem dos planetas no array `planetAudioNames` deve corresponder à ordem no `XRRigMover.targets`

2. **Nomes dos Arquivos**: Os nomes dos arquivos de áudio são case-sensitive no código, mas o Unity geralmente é case-insensitive. Ainda assim, é recomendado usar os nomes exatamente como especificado.

3. **Performance**: Os áudios são carregados dinamicamente usando `Resources.Load()`, o que é eficiente para este uso.

4. **Múltiplos Planetas**: Cada planeta deve ter seu próprio componente `PlanetQuiz` e `AudioSource`.

5. **Reset do Quiz**: Quando o usuário avança para o próximo planeta, o estado do quiz é resetado automaticamente, incluindo o estado do áudio.

## Exemplo de Configuração Completa

1. **GameObject do Planeta (ex: "Terra")**:
   - Componente `PlanetIdentifier`:
     - Planet Index: 2
   - Componente `AudioSource`:
     - Play On Awake: false
     - Loop: false
     - Volume: 0.8
   - Filho: Canvas do Quiz
     - Componente `PlanetQuiz`:
       - Quiz Canvas: (arrastar o Canvas)
       - Audio Source: (arrastar o AudioSource do planeta)
       - Replay Audio Button: (arrastar o botão de repetir)
       - Question Panels: (configurar os painéis)
       - Feedback Text: (arrastar o texto de feedback)
       - Next Planet Button: (arrastar o botão próximo)

## Suporte

Se encontrar problemas não cobertos neste documento, verifique:
1. Os logs do Unity Console para mensagens de erro
2. A configuração dos componentes conforme descrito acima
3. Se todos os arquivos de áudio estão presentes e nomeados corretamente

