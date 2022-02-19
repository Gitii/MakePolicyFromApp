<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/Gitii/MakePolicyFromApp">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Make Policy From App</h3>

  <p align="center">
    A small console application that generates WDAC policy files from any installer.
    The generated policy includes the installer itself and it's contents. Most known exe- and msi-based installers are supported.
    <br />
    <a href="https://github.com/Gitii/MakePolicyFromApp"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Gitii/MakePolicyFromApp/issues">Report Bug</a>·
    <a href="https://github.com/Gitii/MakePolicyFromApp/issues">Request Feature</a>
  </p>
</p>


<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary><h2 style="display: inline-block">Table of Contents</h2></summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li>
      <a href="#getting-started-build-your-own-copy">Getting Started (build your own copy)</a>
      <ul>
        <li><a href="#prerequisites-1">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://github.com/Gitii/MakePolicyFromApp)

A small console application that generates WDAC policy files from any installer.
The generated policy includes the installer itself and it's contents. 
Most known exe- and msi-based installers are supported.

This project utilizes [Universal Extractor 2 (UniExtract2)](https://github.com/Bioruebe/UniExtract2) for extraction 
and the native [New-CIPolicy](https://docs.microsoft.com/en-us/powershell/module/configci/new-cipolicy?view=windowsserver2019-ps) command for policy generation.  

The generated policy includes all executables and dll-files with level `Publisher` and `Hash` as fallback. 
The values of `FriendlyName` & `ID` attributes will be changed for better readability.


### Built With

* [.Net 6](https://dotnet.microsoft.com/download/dotnet/6.0)
* [C# 10](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10)

<!-- GETTING STARTED -->
## Getting Started

To start using `MakePolicyFromApp` as fast as possible, [download](https://github.com/Gitii/MakePolicyFromApp/releases) the latest release from this project page.  
You do not **need** any runtime enviroment because it's a self-contained applications and framework-independent.  

### Prerequisites
* Windows 10 10.0.17763.0 **Pro** or better (WDAC is only supported on PRO or higher)

<!-- USAGE EXAMPLES -->
## Usage

Execute this application from the commandline. There is no GUI-support.  

```
MakePolicyFromApp 1.0.0
Copyright (c) 2021 Gitii

  generate    Analyzes the installer and generates a WDAC policy file.

  analyze     Analyzes the installer and prints the content to stdout.

  help        Display more information on a specific command.

  version     Display version information.
```

### generate
```
MakePolicyFromApp 1.0.0
Copyright (c) 2021 Gitii

  -i, --input     Required. The installer that will be analyzed.

  -n, --name      Friendly name for policy: If not set, the name will be derived from the installer.

  -o, --output    Required. The output file path to the policy file. If not set, the content will be printed to stdout.

  --help          Display this help screen.

  --version       Display version information.

```

#### Example
Generate policy for 7-zip 21.03:  
```sh
.\MakePolicyFromApp.exe generate -i "%UserProfile%\Downloads\7z2103-x64.exe" -o "%UserProfile%\Downloads\7z2103-x64.xml"
```
That will generate the policy file:
```xml
<SiPolicy xmlns="urn:schemas-microsoft-com:sipolicy">
  <VersionEx>10.0.0.0</VersionEx>
  <PlatformID>{2E07F7E4-194C-4D20-B7C9-6F44A6C5A234}</PlatformID>
  <Rules>
    <Rule>
      <Option>Enabled:Unsigned System Integrity Policy</Option>
    </Rule>
    <Rule>
      <Option>Enabled:Audit Mode</Option>
    </Rule>
    <Rule>
      <Option>Enabled:Advanced Boot Options Menu</Option>
    </Rule>
    <Rule>
      <Option>Required:Enforce Store Applications</Option>
    </Rule>
    <Rule>
      <Option>Enabled:UMCI</Option>
    </Rule>
  </Rules>
  <!--EKUS-->
  <EKUs />
  <!--File Rules-->
  <FileRules>
    <Allow ID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/installer/7z2103-x64.exe Hash Sha1" Hash="3656BD07B69BAE0B139D8D7AE46A9550E1E678B5" />
    <Allow ID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/installer/7z2103-x64.exe Hash Sha256" Hash="F4FF3236EF9FA0857B3411217AE3792CAB604DC8223AE5069A06E772A6ACBCF5" />
    <Allow ID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/installer/7z2103-x64.exe Hash Page Sha1" Hash="CB86E9406C5AB8976980CD5661FA9FCA5E19DB49" />
    <Allow ID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/installer/7z2103-x64.exe Hash Page Sha256" Hash="9EE089B041A2E38EC0B5D11611B11EDE5E95DF79D06AA6BB3FAC71A4631C9201" />
    <Allow ID="ID_ALLOW_APP_7_ZIP_DLL_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip.dll Hash Sha1" Hash="D2F64880AA0777D4D790E779E03CEBF063BF9D8C" />
    <Allow ID="ID_ALLOW_APP_7_ZIP_DLL_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip.dll Hash Sha256" Hash="6F5DB7517C28F7C1F9902B7CA87B8C801D951B4CDB1524F7BFC40BD6E4222FB1" />
    <Allow ID="ID_ALLOW_APP_7_ZIP_DLL_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip.dll Hash Page Sha1" Hash="B8591FDBF3D9E55A5E510EB92CB68610F63EAC19" />
    <Allow ID="ID_ALLOW_APP_7_ZIP_DLL_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip.dll Hash Page Sha256" Hash="F71E5AC4A5223C5A35E82F6DDDE6E850BDEBE528BAD0068780ACBBEE29FABFA3" />
    <Allow ID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip32.dll Hash Sha1" Hash="86E44F26AE05627688824D953D65707B98DC7C6B" />
    <Allow ID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip32.dll Hash Sha256" Hash="AD6799C9B7075BAAE284C6DE932E34135C797750393759282D2ECC2CE66106B7" />
    <Allow ID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip32.dll Hash Page Sha1" Hash="D975B4A1C16DB7004F8D78CFC004739010D7042E" />
    <Allow ID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7-zip32.dll Hash Page Sha256" Hash="6F9B47BDDF28660A0CB72977B629798544E1F6A378CA334B7ACE6558261A6A46" />
    <Allow ID="ID_ALLOW_APP_7Z_DLL_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7z.dll Hash Sha1" Hash="3732F42E8BFE9F61EDD74A8538068B8CD3D52020" />
    <Allow ID="ID_ALLOW_APP_7Z_DLL_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7z.dll Hash Sha256" Hash="13FD37D47A5D484BE81A8E7FF48EE381613F5E4A523D9319A49EBA48C6464446" />
    <Allow ID="ID_ALLOW_APP_7Z_DLL_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7z.dll Hash Page Sha1" Hash="32AB619A08FD6EF6ED6A2D8AF25221C898C64E44" />
    <Allow ID="ID_ALLOW_APP_7Z_DLL_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7z.dll Hash Page Sha256" Hash="A274117B0444362B9FAC837DE7BFCF0BD72ABD018BB42D6AA41A254C0453A4F7" />
    <Allow ID="ID_ALLOW_APP_7Z_EXE_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7z.exe Hash Sha1" Hash="A52A499DA618C8718416B7FE501594084B5BF567" />
    <Allow ID="ID_ALLOW_APP_7Z_EXE_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7z.exe Hash Sha256" Hash="6D6DCB50290182A27707B98C2BFE2C00B55125618BF349854D654A0060840775" />
    <Allow ID="ID_ALLOW_APP_7Z_EXE_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7z.exe Hash Page Sha1" Hash="B513DCBAB6F0F32E8704EBAEB374D8AD2BEB597A" />
    <Allow ID="ID_ALLOW_APP_7Z_EXE_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7z.exe Hash Page Sha256" Hash="5A47C5479DD86F4B0DCF43B936F5219D703427BFF4BD9628C6291DAA56D99B3B" />
    <Allow ID="ID_ALLOW_APP_7Z_SFX_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7z.sfx Hash Sha1" Hash="F2468F7FE47659790F331CAF5BB4665A89E9E4B1" />
    <Allow ID="ID_ALLOW_APP_7Z_SFX_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7z.sfx Hash Sha256" Hash="614D8154E1AD2155ACF732F809FBC8D93737DF88A05B182F1911BF297C4EE326" />
    <Allow ID="ID_ALLOW_APP_7Z_SFX_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7z.sfx Hash Page Sha1" Hash="0ED5F24AEBF7CC2C79F9CD8D228F2314FD88796B" />
    <Allow ID="ID_ALLOW_APP_7Z_SFX_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7z.sfx Hash Page Sha256" Hash="3DF452D729C9638238B8BA6ADC05794702E6B8416421BDDCF3B6E86B8BD2F56C" />
    <Allow ID="ID_ALLOW_APP_7ZCON_SFX_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7zCon.sfx Hash Sha1" Hash="F2CCBFB2CD892343118BA794A2A22E5C6490BF00" />
    <Allow ID="ID_ALLOW_APP_7ZCON_SFX_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7zCon.sfx Hash Sha256" Hash="93F7013C34F0EE56C5F993018B8E49E9771B838CDC0A13C3E7A337AD7F71126F" />
    <Allow ID="ID_ALLOW_APP_7ZCON_SFX_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7zCon.sfx Hash Page Sha1" Hash="083D96E6A7BAE84D0A22C7D77666790B88991700" />
    <Allow ID="ID_ALLOW_APP_7ZCON_SFX_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7zCon.sfx Hash Page Sha256" Hash="3BDA4F5FA1885AB763A12A9BD646AC41E4A87B7C755E425487254176F7A35935" />
    <Allow ID="ID_ALLOW_APP_7ZFM_EXE_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7zFM.exe Hash Sha1" Hash="A16B9E7E853777D0454A1FC20CF361B47A643FCF" />
    <Allow ID="ID_ALLOW_APP_7ZFM_EXE_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7zFM.exe Hash Sha256" Hash="5AA58111C8C73CDAE2059D072FE31A8A5615D95AF834D9FEA39E0652F34EFC29" />
    <Allow ID="ID_ALLOW_APP_7ZFM_EXE_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7zFM.exe Hash Page Sha1" Hash="AA387FD2D5F8BBAD1447F7116241178545D13CAE" />
    <Allow ID="ID_ALLOW_APP_7ZFM_EXE_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7zFM.exe Hash Page Sha256" Hash="97B7BCACD9A2CD2012C73DBD47269FC6D60F973B07699888971A6249668F8B72" />
    <Allow ID="ID_ALLOW_APP_7ZG_EXE_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7zG.exe Hash Sha1" Hash="AE88FF5A69C2210B00CB8D0678E55A0013CB9F7C" />
    <Allow ID="ID_ALLOW_APP_7ZG_EXE_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7zG.exe Hash Sha256" Hash="29667EFFE2655D8C754540150BFE6FE9031F43469CF00909903B6839E7947837" />
    <Allow ID="ID_ALLOW_APP_7ZG_EXE_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/7zG.exe Hash Page Sha1" Hash="2E16B86E75DC1A58124F22DA0424CA4CE1A0D5BA" />
    <Allow ID="ID_ALLOW_APP_7ZG_EXE_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/7zG.exe Hash Page Sha256" Hash="3295F54CDC8417C2F0B5E2688F50CF197E6A1D1FAB3575817E5656E1E780ABF2" />
    <Allow ID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/Uninstall.exe Hash Sha1" Hash="2F2D12810BE397D2B1FD7442A183BCDEBEF6790B" />
    <Allow ID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/Uninstall.exe Hash Sha256" Hash="73EC8914F246B2E33B24292EE95781C55CA192ABC1ADC60DE049FCED611FF559" />
    <Allow ID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_PAGE_SHA1" FriendlyName="[7-Zip (21.03 beta)]/app/Uninstall.exe Hash Page Sha1" Hash="2B3EE40C6634E03FFFF683BE12A3AB631A1E2FC2" />
    <Allow ID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_PAGE_SHA256" FriendlyName="[7-Zip (21.03 beta)]/app/Uninstall.exe Hash Page Sha256" Hash="AEE67CD1E7A85AC2B07594AF26D42F88CB5908BCE2950BB1558E115167CE37F1" />
  </FileRules>
  <!--Signers-->
  <Signers />
  <!--Driver Signing Scenarios-->
  <SigningScenarios>
    <SigningScenario Value="131" ID="ID_SIGNINGSCENARIO_DRIVERS_1" FriendlyName="Generated policy for &quot;7-Zip (21.03 beta)&quot; (10.09.2021)">
      <ProductSigners />
    </SigningScenario>
    <SigningScenario Value="12" ID="ID_SIGNINGSCENARIO_WINDOWS" FriendlyName="Generated policy for &quot;7-Zip (21.03 beta)&quot; (10.09.2021)">
      <ProductSigners>
        <FileRulesRef>
          <FileRuleRef RuleID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_INSTALLER_7Z2103_X64_EXE_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP_DLL_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP_DLL_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP_DLL_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP_DLL_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7_ZIP32_DLL_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_DLL_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_DLL_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_DLL_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_DLL_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_EXE_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_EXE_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_EXE_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_EXE_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_SFX_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_SFX_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_SFX_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7Z_SFX_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZCON_SFX_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZCON_SFX_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZCON_SFX_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZCON_SFX_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZFM_EXE_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZFM_EXE_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZFM_EXE_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZFM_EXE_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZG_EXE_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZG_EXE_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZG_EXE_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_7ZG_EXE_HASH_PAGE_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_SHA256" />
          <FileRuleRef RuleID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_PAGE_SHA1" />
          <FileRuleRef RuleID="ID_ALLOW_APP_UNINSTALL_EXE_HASH_PAGE_SHA256" />
        </FileRulesRef>
      </ProductSigners>
    </SigningScenario>
  </SigningScenarios>
  <UpdatePolicySigners />
  <CiSigners />
  <HvciOptions>0</HvciOptions>
  <PolicyTypeID>{A244370E-44C9-4C06-B551-F6016E563076}</PolicyTypeID>
</SiPolicy>
```

### analyze
```
MakePolicyFromApp 1.0.0
Copyright (c) 2021 Gitii

  -i, --input    Required. The installer that will be analyzed.

  --help         Display this help screen.

  --version      Display version information.

```

#### Example
Analyze the installer of 7-zip 21.03:  
```sh
.\MakePolicyFromApp.exe analyze -i "%UserProfile%\Downloads\7z2103-x64.exe"
```
That will generate the output (on stdout):
```
--------------------------------------------
| Result        | FileName                 |
--------------------------------------------
| FileNotSigned | app\7-zip.dll            |
--------------------------------------------
| FileNotSigned | app\7-zip32.dll          |
--------------------------------------------
| FileNotSigned | app\7z.dll               |
--------------------------------------------
| FileNotSigned | app\7z.exe               |
--------------------------------------------
| FileNotSigned | app\7zFM.exe             |
--------------------------------------------
| FileNotSigned | app\7zG.exe              |
--------------------------------------------
| FileNotSigned | app\Uninstall.exe        |
--------------------------------------------
| FileNotSigned | installer\7z2103-x64.exe |
--------------------------------------------

Count: 8
```

<!-- ROADMAP -->
## Roadmap
`DONE` Main feature: Generation of policies  
`OPEN` Quality of life: Logging level

See the [open issues](https://github.com/Gitii/MakePolicyFromApp/issues) for a list of proposed features (and known issues).


## Getting Started (build your own copy)

To get a local copy up and running follow these simple steps.

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* Download and install [.Net SDK](https://www.microsoft.com/net/download/) (which includes [dotnet-cli](https://docs.microsoft.com/de-de/nuget/consume-packages/install-use-packages-dotnet-cli))
* Visual Studio 2022 (Preview)  

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/Gitii/MakePolicyFromApp.git
   ```
2. Install nuget packages (optional, Visual Studio will do that for you)
   ```sh
   dotnet restore
   ```

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Project Link: [https://github.com/Gitii/MakePolicyFromApp](https://github.com/Gitii/MakePolicyFromApp)

<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements

* [Universal Extractor 2 (UniExtract2)](https://github.com/Bioruebe/UniExtract2)
* [CommandLineParser](https://github.com/commandlineparser/commandline)


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/Gitii/MakePolicyFromApp
[contributors-url]: https://github.com/Gitii/MakePolicyFromApp/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Gitii/MakePolicyFromApp
[forks-url]: https://github.com/Gitii/MakePolicyFromApp/network/members
[stars-shield]: https://img.shields.io/github/stars/Gitii/MakePolicyFromApp
[stars-url]: https://github.com/Gitii/MakePolicyFromApp/stargazers
[issues-shield]: https://img.shields.io/github/issues/Gitii/MakePolicyFromApp
[issues-url]: https://github.com/Gitii/MakePolicyFromApp/issues
[license-shield]: https://img.shields.io/github/license/Gitii/MakePolicyFromApp
[license-url]: https://github.com/Gitii/MakePolicyFromApp/blob/master/LICENSE