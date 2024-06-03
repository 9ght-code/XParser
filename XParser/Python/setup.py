from cx_Freeze import setup, Executable


setup(
    name="XParser",
    version="1.0",
    author="9ght",
    description="None",
    options = {'build_exe': {'packages':['pandas', 'openpyxl']}},
    executables=[Executable('Converter.py')])