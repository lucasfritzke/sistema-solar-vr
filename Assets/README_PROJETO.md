# 🌌 Sistema Solar VR - Documentação do Projeto

## 📖 Visão Geral

Este projeto implementa uma experiência imersiva em Realidade Virtual usando Google Cardboard, onde o usuário faz uma jornada pelo Sistema Solar. Em cada planeta, o usuário ouve uma narração explicativa e responde um quiz. A interação é feita através de raycast/gaze (olhar fixo por alguns segundos).

---

## 📚 Documentação de Configuração

**Para configurar o projeto passo a passo, consulte:**
- **`SETUP_INSTRUCOES.md`** - Instruções detalhadas e específicas de configuração

Este documento contém instruções passo a passo com:
- ✅ Exatamente onde clicar
- ✅ O que digitar em cada campo
- ✅ Como adicionar cada componente
- ✅ Três métodos diferentes para adicionar Cardboard
- ✅ Screenshots descritivos em texto
- ✅ Troubleshooting detalhado

---

## 🎯 Funcionalidades Implementadas

✅ **Sistema de Gaze/Raycast**
- Reticle visual no centro da tela
- Preenchimento progressivo ao olhar para objetos interativos
- Suporte para objetos 3D e UI
- Tempo configurável de gaze (padrão: 2 segundos)

✅ **Sistema de Câmera**
- Posicionamento inicial no Sol
- Transição suave entre planetas
- Movimento animado com curva de easing

✅ **Sistema de Áudio**
- Áudio explicativo para cada planeta (máximo 1 minuto)
- Reprodução automática ao chegar no planeta
- Botão para repetir áudio

✅ **Sistema de Quiz**
- Pergunta por planeta
- Duas alternativas (A e B)
- Feedback visual imediato (correto/errado)
- Permite nova tentativa ao errar
- Botão "Próximo" habilitado apenas após acertar

✅ **Integração Completa**
- Fluxo automático: Câmera → Áudio → Quiz → Próximo Planeta
- Gerenciamento centralizado em `SolarVRManager`

---

## 📁 Estrutura de Scripts

### Scripts Principais

1. **`SolarVRManager.cs`**
   - Gerenciador principal do sistema
   - Controla fluxo completo: câmera, áudio, quiz
   - Gerencia transições entre planetas
   - **Localização**: `Assets/scripts/SolarVRManager.cs`

2. **`SolarVRGaze.cs`**
   - Sistema de raycast/gaze
   - Detecta objetos ao olhar
   - Gerencia reticle visual
   - Suporta objetos 3D e UI
   - **Localização**: `Assets/scripts/SolarVRGaze.cs`

3. **`CameraSmoothMovement.cs`**
   - Movimento suave da câmera
   - Transições animadas
   - **Localização**: `Assets/scripts/CameraSmoothMovement.cs`

4. **`SelectableObject.cs`**
   - Componente para objetos interativos
   - Feedback visual (cor muda ao olhar)
   - **Localização**: `Assets/scripts/SelectableObject.cs`

5. **`StartJourneyButton.cs`**
   - Botão/inicializador da jornada
   - Pode iniciar automaticamente ou por botão
   - **Localização**: `Assets/scripts/StartJourneyButton.cs`

### Scripts de Sistema Solar

6. **`SolarSystemRealistic.cs`**
   - Gerencia rotação dos planetas
   - Gerencia órbitas das luas
   - Planetas ficam fixos (não orbitam o Sol)
   - **Localização**: `Assets/scripts/SolarSystemRealistic.cs`

---

## 🚀 Início Rápido

1. **Siga as instruções detalhadas em `SETUP_INSTRUCOES.md`**

2. **Resumo dos passos principais:**
   - Configurar Cardboard (Project Settings)
   - Configurar Main Camera com scripts
   - Criar Reticle UI
   - Posicionar planetas
   - Criar quizzes
   - Configurar SolarVRManager

---

## 🎮 Fluxo do Usuário

1. **Início**
   - Câmera posicionada no Sol
   - Opção: Botão "Iniciar Jornada" ou início automático

2. **Para Cada Planeta:**
   
   a. **Movimento da Câmera**
   - Transição suave até o planeta
   - Duração: ~1-2 segundos
   
   b. **Áudio Explicativo**
   - Toca automaticamente ao chegar
   - Duração máxima: 1 minuto
   - Botão "Repetir Áudio" disponível
   
   c. **Quiz**
   - Aparece após áudio terminar
   - Mostra pergunta e 2 alternativas
   - Usuário olha para alternativa (gaze/raycast)
   - Feedback imediato: ✅ Correto / ❌ Errado
   
   d. **Se Errar:**
   - Permite nova tentativa
   - Quiz permanece visível
   
   e. **Se Acertar:**
   - Botão "Próximo" aparece
   - Habilitado para interação
   - Usuário olha para "Próximo" (gaze)
   - Câmera vai para próximo planeta

3. **Final**
   - Após último planeta, mostra mensagem de conclusão

---

## 🔧 Configurações Importantes

### Tags e Layers

- **Tag `Selectable`**: Todos os botões interativos devem ter esta tag
- **Layers**: Verificar que objetos UI estão em layers incluídos no `Interaction Layers` do `SolarVRGaze`

### Canvas Configuration

- **Render Mode**: `World Space`
- **Event Camera**: Main Camera (Cardboard)
- **Position**: Ao lado do planeta (não à frente)

### Audio Configuration

- **AudioSource**: Um único AudioSource compartilhado
- **AudioClips**: Um por planeta (máximo 1 minuto)
- **Play On Awake**: false

### Gaze Configuration

- **Gaze Time**: 2-3 segundos (recomendado)
- **Raycast Distance**: 100 unidades
- **Interaction Layers**: Todos (Everything) ou layers específicos

---

## 🐛 Troubleshooting Rápido

### Raycast não funciona
- ✅ Verificar tag `Selectable` nos botões
- ✅ Verificar `Box Collider` em botões UI
- ✅ Verificar `Interaction Layers` no `SolarVRGaze`
- ✅ Verificar se `SolarVRGaze` está na câmera

### Áudio não toca
- ✅ Verificar se `AudioSource` está configurado
- ✅ Verificar se `AudioClip` está atribuído
- ✅ Verificar volume do `AudioSource`

### Quiz não aparece
- ✅ Verificar se `Quiz Canvas` está atribuído
- ✅ Verificar ordem: Áudio deve tocar primeiro
- ✅ Verificar logs do console

### Botão Próximo não habilita
- ✅ Verificar se resposta correta está configurada (0 ou 1)
- ✅ Verificar logs do console ao responder
- ✅ Verificar se `quizActive` está true

**Para troubleshooting detalhado, consulte `SETUP_INSTRUCOES.md`**

---

## 📚 Referências

- **Asset Store**: The Solar System (Unity Asset Store)
- **Unity Version**: 6000.0.29f1
- **VR Plugin**: Google Cardboard XR Plugin for Unity

---

## 👥 Autores

Projeto desenvolvido para a disciplina de Realidade Virtual.

---

## 📄 Licença

Projeto acadêmico - Uso educacional.

---

**⚠️ IMPORTANTE: Consulte `SETUP_INSTRUCOES.md` para instruções detalhadas de configuração passo a passo!**

