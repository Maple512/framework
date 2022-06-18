# ![One Framework](./logo.png)

[![build](https://github.com/one-land/framework/actions/workflows/build.yml/badge.svg)](https://github.com/one-land/framework/actions/workflows/build.yml)
[![publish](https://github.com/one-land/framework/actions/workflows/publish.yml/badge.svg)](https://www.nuget.org/packages?q=tags%3A%22one-land+one+one-framework+framework%22+maple512)
![code line](https://img.shields.io/tokei/lines/github/one-land/framework?style=flat)
![release](https://img.shields.io/github/v/release/one-land/framework?include_prereleases&style=flat)
![license](https://img.shields.io/github/license/one-land/framework)

> `One Framework`，一个基础设施框架

> 协作，统一

## 代码规范

- `OneF`：作为整个项目的前缀名
- `able`：子项目后缀统一加`able`
- `_Test`: 单元测试以`_Test`为后缀
- `DateTimeOffset`：时间统一使用`DateTimeOffset`
- `Info`：只包含属性的类（非实体与DTO），使用`Info`作为后缀
- `Abstractions`：抽象项目，无具体实现，其中包括具体项目所需的`Attribute`，`Interface`等

## 项目

- `event`: 事件，进程内通信
- `infrastructure`: 基础设施
- `main`: 基础项目，所有项目都需要引用的一个

## 参考

### 框架/项目

- `dotnet`：<https://docs.microsoft.com/zh-cn/dotnet/>
- `efcore`：<https://docs.microsoft.com/zh-cn/ef/core/>
- `abp`：<https://github.com/abpframework>
- `aspnetboilerplate`：<https://github.com/aspnetboilerplate/aspnetboilerplate>
- `furion`：<https://dotnetchina.gitee.io/furion/docs>
- `osharp`：<https://github.com/dotnetcore/osharp>

### 文章/博客

- `lindexi`：<https://lindexi.gitee.io/>
