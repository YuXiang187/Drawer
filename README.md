# Drawer

一个简易的名称抽取程序。

如需使用Android版本，请前往[Drawer-Android](https://github.com/YuXiang187/Drawer-Android)库。

如需使用Linux版本，请前往[Drawer-Linux](https://github.com/YuXiang187/Drawer-Linux)库。

## 功能

编辑列表的语法为：

```
名称1,名称2,名称3,名称4,名称5,...
```

注意：分割符为**英文逗号**，不是中文逗号。

程序首次启动时默认使用<kbd>F8</kbd>键进行抽取，你可以通过系统托盘菜单中的“设置”功能来更改热键。

## 抽取算法

自v4.1版本起，抽取名称功能的 Gaussian（高斯分布）模型参考了 [rpick](https://github.com/bowlofeggs/rpick) 的实现

该算法会根据抽签历史动态调整概率：名单中越久没有被抽中的项目，概率越高；最近被抽中的项目移动到列表末尾，概率降低。默认标准差缩放因子为 3.0

本项目与 rpick 均采用 GPL-3.0 开源许可证

## 构建与打包

启动 Visual Studio 生成解决方案，使用 Inno setup 7 构建安装包

Inno setup 7 配置文件：

```ini
[Setup]
AppId={{61CD1A78-4BF1-4B0A-83B2-63F7385E7E86}
AppName=YuXiang Drawer
AppVersion=4.1
AppPublisher=YuXiang187
DefaultDirName={autopf}\Drawer
UninstallDisplayIcon={app}\Drawer.exe
AppMutex=Drawer
;TODO: change architectures
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
;ArchitecturesAllowed=arm64
;ArchitecturesInstallIn64BitMode=arm64
;ArchitecturesAllowed=x86compatible
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
CloseApplications=yes
;TODO: output dir
OutputDir=C:\Users\yuxia\Desktop\inno
;TODO: output filename
OutputBaseFilename=Drawer_4.1_x64_setup
;TODO: icon file path
SetupIconFile=C:\Users\yuxia\Desktop\files\icon.ico
SolidCompression=yes
WizardStyle=modern light

[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
;TODO: origin exe file
Source: "C:\Users\yuxia\Desktop\release\Drawer_4.1_x64\Drawer.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\YuXiang Drawer"; Filename: "{app}\Drawer.exe"
Name: "{autodesktop}\YuXiang Drawer"; Filename: "{app}\Drawer.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\Drawer.exe"; Description: "{cm:LaunchProgram,YuXiang Drawer}"; Flags: nowait postinstall skipifsilent

[Code]
procedure CurUninstallStepChanged(UninstallStep: TUninstallStep);
begin
  if UninstallStep = usPostUninstall then
  begin
    RegDeleteValue(
      HKEY_CURRENT_USER,
      'Software\Microsoft\Windows\CurrentVersion\Run',
      'Drawer'
    );
    if FileExists(ExpandConstant('{app}\Drawer.config')) then
    begin
      if MsgBox(
        '是否删除 YuXiang Drawer 的配置文件？' + #13#10#13#10 +
        '如果以后重新安装，可以保留此文件以恢复之前的设置。',
        mbConfirmation,
        MB_YESNO
      ) = IDYES then
      begin
        DeleteFile(ExpandConstant('{app}\Drawer.config'));
      end;
    end;
    RemoveDir(ExpandConstant('{app}'));
  end;
end;
```

## 更新日志

**4.1版本**（最新）：

* 修复了一些Bug
* 抽取算法从不放回抽样改为高斯分布抽样
* 移除背景图片支持

**4.0版本**：

* 修复了一些Bug
* 列表存储位置更改为Drawer.config
* 自定义密码功能
* 自定义热键功能
* 自定义列表功能

**3.3版本**：

* 修复了一些Bug
* 配置文件Drawer.config的语法已更新
* 支持读取自定义密钥的list文件
* 添加统计窗口
* 更新软件库

**3.2版本**：

* 修复了一些Bug
* 添加了“关于”弹窗

**3.1版本**：

* 修复了开机无法自动启动的Bug
* 优化了部分资源文件

**3.0版本**：

* 程序改用C#语言编写
* 主窗体支持添加背景图片
* 存储名称的文件后缀名更改为`.txt`

**2.5版本**：

* 修复了一些Bug
* 优化了系统托盘菜单

**2.4版本**：

* 优化了“浮窗”的功能
* 现在能保存软件设置了

**2.3版本**：

* 修复了一些Bug
* 添加了“浮窗”的功能

**2.2版本**：

* 修复了一些Bug
* FlatLaf库版本更新至3.2.1
* 抽取名称后自动保存已经抽取过的名称至`pool.es`文件
* 系统托盘菜单删除“重置”、“保存”两个针对`pool.es`文件操作的菜单项

**2.1版本**：

* FlatLaf库版本更新至3.1.1
* 在主窗体底部添加了关闭窗口的倒计时进度条
* 重写了程序架构，提升了程序的运行速度

**2.0版本**：

* 加入了每轮不重复抽取名称的算法
* 每隔10分钟自动保存已经抽取过的名称至`pool.es`文件
* 系统托盘菜单添加“重置”、“保存”两个针对`pool.es`文件操作的菜单项