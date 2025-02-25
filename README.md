# TextLockAndDropIt

## Overview
TextLockAndDropIt is a simple Windows utility that allows users to enter multiple text values, lock them, and quickly copy/paste them into external applications with minimal clicks. The application stays on top of other windows and supports configurable text box counts.

## Installation
### **1. Clone the repository**
Open a terminal or command prompt and run:
```sh
 git clone https://github.com/your-repo.git
```

### **2. Navigate to the project folder**
```sh
cd your-repo
```

### **3. Run the install script**
Execute the following command to build the application and move the EXE to the main folder for easy access:
```sh
install.bat
```
This will compile the application and place the executable in the root directory.

## Usage
### **Running the Application**
After installation, simply **double-click the EXE** located in the main project folder.

### **How It Works**
1. **Enter values** in the text boxes.
2. **Lock the fields** by clicking the `Lock` button (this makes them read-only and enables the quick-copy feature).
3. **Click on a text box** to copy its value.
4. **Click into an external application** (e.g., a browser or form field) to **automatically paste** the copied value.
5. **Change the number of fields** using the dropdown (persisted across sessions).
6. **Unlock** to edit values as needed.

### **Features**
- **Persistent Storage**: Saves text field values and field count.
- **Quick Copy & Paste**: Click a locked field to copy, then click externally to paste.
- **Always On Top**: Keeps the utility accessible while testing.
- **Adjustable Field Count**: Modify the number of text fields dynamically.

## Troubleshooting
- If the EXE does not appear after installation, ensure that `.NET Runtime` is installed.
- If auto-paste does not work, try running the application as an administrator.

## License
This project is open-source and free to use.
