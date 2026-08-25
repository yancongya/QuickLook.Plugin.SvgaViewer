# Quick packaging script (run from project root)
$ErrorActionPreference = "Stop"

Remove-Item QuickLook.Plugin.PagViewer.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path bin/Release -Exclude "*.pdb", "*.xml"
Compress-Archive -Path $files -DestinationPath "QuickLook.Plugin.PagViewer.zip"
Move-Item "QuickLook.Plugin.PagViewer.zip" "QuickLook.Plugin.PagViewer.qlplugin"

Write-Host "Done: QuickLook.Plugin.PagViewer.qlplugin" -ForegroundColor Green
