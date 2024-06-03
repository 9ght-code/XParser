
import pandas as pd
import os

converted = []

for root, subfolder, files in os.walk(f"{os.curdir}\\output"):
    for file in files:
     file_to_convert = f"{root}\\{file}"
     
     try:
         
         if file.split('.')[1] != "xlsx" or file.split('.')[1] != "xls":
             if file.split('.')[0] not in converted:
                 df = pd.read_csv(file_to_convert, sep='#', index_col=1)
                 df.to_excel(f'{root}\\{file.split('.')[0]}.xlsx', 'Sheet1', index = False)
                  
         else:
            converted.append(file.split('.')[0])
       
     except Exception as ex: continue