# Drawer

一个简易的名称抽取程序。

在程序根目录下必须要有`list.txt`文件才能启动此程序。此文件为**加密**文件，请使用[CipherPad](https://github.com/YuXiang187/CipherPad)编辑。

如需使用2.5以下的版本，请前往[Drawer-Java](https://github.com/YuXiang187/Drawer-Java)库。

`list.txt`文件语法为：

```
名称1,名称2,名称3,名称4,名称5,...
```

注意：分割符为**英文逗号**，不是中文逗号。

在`app.manifest`文件中修改`requestedExecutionLevel`以不同身份启动程序：

* `asInvoker`：以普通用户身份启动程序
* `requireAdministrator`：以管理员身份启动程序

---

程序会根据`list.txt`文件提供的名称进行随机抽取。

程序首次启动时默认开启热键<kbd>Ctrl</kbd>，你可以通过系统托盘菜单来选择是否开启该热键。

程序首次启动时默认不显示一个带启动按钮的浮窗，你也可以通过系统托盘菜单来选择是否显示该浮窗。

程序首次启动时默认不开机自动启动，你可以通过系统托盘菜单来选择是否开机自动启动该程序。

---

名称抽取结束后，可以按下窗体任意位置关闭窗口显示，也可以等待主窗体底部的倒计时进度条的值为0%时让程序自动关闭窗口显示。

或者，你也可以选择继续随机抽取名称，让窗口继续显示。

---

本软件支持设置背景图片。最小的图片大小为450x250（比例9:5）。

背景图片的名称为以下的任意一种：

* background.jpg
* background.jpeg
* background.png
* background.bmp

---

**3.3版本更新日志**（最新）：

* 修复了一些Bug
* 配置文件Drawer.config的语法已更新
* 支持读取自定义密钥的list
* 添加统计窗口
* 更新软件库

**3.2版本更新日志**：

* 修复了一些Bug
* 添加了“关于”弹窗

**3.1版本更新日志**：

* 修复了开机无法自动启动的Bug
* 优化了部分资源文件

**3.0版本更新日志**：

* 程序改用C#语言编写
* 主窗体支持添加背景图片
* 存储名称的文件后缀名更改为`.txt`

**2.5版本更新日志**：

* 修复了一些Bug
* 优化了系统托盘菜单

**2.4版本更新日志**：

* 优化了“浮窗”的功能
* 现在能保存软件设置了

**2.3版本更新日志**：

* 修复了一些Bug
* 添加了“浮窗”的功能

**2.2版本更新日志**：

* 修复了一些Bug
* FlatLaf库版本更新至3.2.1
* 抽取名称后自动保存已经抽取过的名称至`pool.es`文件
* 系统托盘菜单删除“重置”、“保存”两个针对`pool.es`文件操作的菜单项

**2.1版本更新日志**：

* FlatLaf库版本更新至3.1.1
* 在主窗体底部添加了关闭窗口的倒计时进度条
* 重写了程序架构，提升了程序的运行速度

**2.0版本更新日志**：

* 加入了每轮不重复抽取名称的算法
* 每隔10分钟自动保存已经抽取过的名称至`pool.es`文件
* 系统托盘菜单添加“重置”、“保存”两个针对`pool.es`文件操作的菜单项
