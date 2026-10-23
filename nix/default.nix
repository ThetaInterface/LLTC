{ lib, buildDotnetModule, dotnetCorePackages, fetchFromGitHub }:

buildDotnetModule rec {
    pname = "LLTC";
    version = "1.3";

    src = fetchFromGitHub {
        owner = "ThetaInterface";
        repo = "LLTC";
        rev = "44924710dd43cd47ebad56c9f22f44e5cffb1382";
        sha256 = "sha256-nvs49DnE/pE5odULjafFbSAUiX0EoZCv1U6/2EoJjpg=";
    };

    projectFile = "LLTC.csproj";

    dotnet-sdk = dotnetCorePackages.sdk_10_0;
    dotnet-runtime = dotnetCorePackages.runtime_10_0;

    meta = with lib; {
        description = "LLTC";
        homepage = "https://github.com/ThetaInterface/LLTC";
        license = licenses.gpl3Only;
        platforms = platforms.linux;
        mainProgram = "LLTC";
    };
}