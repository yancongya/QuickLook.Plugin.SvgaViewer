# Quick packaging script (run from project root)
$ErrorActionPreference = "Stop"

Remove-Item QuickLook.Plugin.SvgaViewer.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path bin/Release -Exclude "*.pdb", "*.xml"
Compress-Archive -Path $files -DestinationPath "QuickLook.Plugin.SvgaViewer.zip"
Move-Item "QuickLook.Plugin.SvgaViewer.zip" "QuickLook.Plugin.SvgaViewer.qlplugin"

Write-Host "Done: QuickLook.Plugin.SvgaViewer.qlplugin" -ForegroundColor Green
