Unicode True
Name "JobbKalk"
OutFile "..\artifacts\JobbKalk-Setup-x64.exe"
InstallDir "$PROGRAMFILES64\JobbKalk"
InstallDirRegKey HKLM "Software\JobbKalk" "InstallDir"
RequestExecutionLevel admin
SetCompressor /SOLID lzma

VIProductVersion "0.1.0.0"
VIAddVersionKey "ProductName" "JobbKalk"
VIAddVersionKey "FileDescription" "JobbKalk installasjonsprogram"
VIAddVersionKey "FileVersion" "0.1.0"

Page directory
Page instfiles
UninstPage uninstConfirm
UninstPage instfiles

Section "JobbKalk" SEC_APP
  SetOutPath "$INSTDIR"
  File "..\artifacts\portable\JobbKalk.exe"
  WriteUninstaller "$INSTDIR\Avinstaller JobbKalk.exe"
  WriteRegStr HKLM "Software\JobbKalk" "InstallDir" "$INSTDIR"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\JobbKalk" "DisplayName" "JobbKalk"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\JobbKalk" "UninstallString" '"$INSTDIR\Avinstaller JobbKalk.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\JobbKalk" "DisplayVersion" "0.1.0"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\JobbKalk" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\JobbKalk" "NoRepair" 1
  CreateDirectory "$SMPROGRAMS\JobbKalk"
  CreateShortcut "$SMPROGRAMS\JobbKalk\JobbKalk.lnk" "$INSTDIR\JobbKalk.exe"
  CreateShortcut "$DESKTOP\JobbKalk.lnk" "$INSTDIR\JobbKalk.exe"
SectionEnd

Section "Uninstall"
  Delete "$DESKTOP\JobbKalk.lnk"
  Delete "$SMPROGRAMS\JobbKalk\JobbKalk.lnk"
  RMDir "$SMPROGRAMS\JobbKalk"
  Delete "$INSTDIR\JobbKalk.exe"
  Delete "$INSTDIR\Avinstaller JobbKalk.exe"
  RMDir "$INSTDIR"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\JobbKalk"
  DeleteRegKey HKLM "Software\JobbKalk"
SectionEnd
