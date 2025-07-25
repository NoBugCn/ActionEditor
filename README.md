﻿# NBC.ActionEdtior
行为时间轴编辑器

能用于项目技能、Buff、剧情等时间轴表现编辑

# 提示

> 由于工作和生活问题，目前已经没有太多精力维护。
>
> 当前2.0版本还有不少UI上bug，但是核心功能基本上完成了！大家有动手能力可以自己修复和扩展，2.0相比1.0上限高很多，代码也更加精简，可读性高很多，功能也多了一些。
>
> 1.0的话相对问题，bug较少，有上线项目的案例。如果您是要用于即将要发布的项目，建议使用1.0。如果时间上不急，那么建议2.0，不过可能需要自己修复一些bug。


# 功能特点
- 理论上适用于`所有类型项目`的技能Buff，战斗镜头，场景效果等等
- 完全自定义扩展，项目只提供核心编辑器脚本，无具体业务逻辑，也就是说，拥有无限可能
- 超高的扩展性，编辑器界面提供大量重写接口。很简单即可自行重写界面
- 很简单的脚本既可完成扩展和开发，一两天即可集成进入自己项目实现技能，buff，场景等可视化编辑
- 还有更多细节请看演示视频和演示项目


# 安装到Unity
安装地址：https://github.com/NoBugCn/ActionEditor.git

不熟悉从url安装package的请看[install from giturl](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

# 功能预览视频
功能预览：https://www.bilibili.com/video/BV1tM4y1q7xg/

# 快速上手
Clone源码到项目中或通过package安装编辑器后，即可开始使用。
点击菜单`NBC/Action Editor/Open Action Editor`打开编辑窗口。
这时编辑器均为空内容。需要自行编辑填充自己需要内容。操作方法可以查看文档

## 【必看】功能示例
https://github.com/NoBugCn/ActionEditorExample

# 文档
操作手册文档：[点击查看](./docs/1.简介.md)

# 交流Q群
567604178

# 更新日志
## 2.0.0
 - 新增List和数组支持
 - 新增自定义类支持
 - 重写新的UI样式
 - 支持Asset自定义绑定Header
 - 新增自定义Group的支持
 - 新增Asset和Group自定义属性支持