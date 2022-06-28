# ![One Framework](./logo.png)

[![Build](https://github.com/maple512/framework/actions/workflows/build.yml/badge.svg)](https://github.com/Maple512/framework/actions/workflows/build.yml)
[![Publish](https://github.com/maple512/framework/actions/workflows/publish.yml/badge.svg)](https://github.com/Maple512/framework/actions/workflows/publish.yml)
![code line](https://img.shields.io/tokei/lines/github/maple512/framework?style=flat)
[![release](https://img.shields.io/github/v/release/maple512/framework?include_prereleases&style=flat&color=blue)](https://github.com/Maple512/framework/releases)
[![license](https://img.shields.io/github/license/maple512/framework)](./LICENSE)

> `One Framework`，一个基础设施框架

## 代码规范

- `OneF`：作为整个项目的前缀名
- `able`：子项目后缀统一加`able`
- `_Test`: 单元测试以`_Test`为后缀
- `DateTimeOffset`：时间统一使用`DateTimeOffset`
- `Info`：只包含属性的类（非实体与DTO），使用`Info`作为后缀
- `Abstractions`：抽象项目，无具体实现，其中包括具体项目所需的`Attribute`，`Interface`等

## 项目

| 名称               | 描述                       | 版本                                                                                                                                                        | 下载量                                                     |
| ------------------ | -------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| `OneF.Utilityable` | 包含一些常用的方法         | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/OneF.Utilityable?label=version&color=blue)](https://www.nuget.org/packages/OneF.Utilityable) | ![Nuget](https://img.shields.io/nuget/dt/OneF.Utilityable) |
| `OneF.Moduleable`  | 为项目提供模块化功能       | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/OneF.Moduleable?label=version&color=blue)](https://www.nuget.org/packages/OneF.Moduleable)   | ![Nuget](https://img.shields.io/nuget/dt/OneF.Moduleable)  |
| `OneF.Eventable`   | 提供本地事件发布、触发功能 | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/OneF.Eventable?label=version&color=blue)](https://www.nuget.org/packages/OneF.Eventable)     | ![Nuget](https://img.shields.io/nuget/dt/OneF.Eventable)   |

## 参考

### 框架/项目

- `dotnet`：<https://docs.microsoft.com/zh-cn/dotnet/>
- `efcore`：<https://docs.microsoft.com/zh-cn/ef/core/>
- `abp`：<https://github.com/abpframework>
- `aspnetboilerplate`：<https://github.com/aspnetboilerplate/aspnetboilerplate>
- `furion`：<https://dotnetchina.gitee.io/furion/docs>
- `osharp`：<https://github.com/dotnetcore/osharp>
