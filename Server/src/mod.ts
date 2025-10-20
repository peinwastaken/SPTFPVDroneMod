/* eslint-disable @typescript-eslint/naming-convention */

// node
import fs from "node:fs";
import path from "node:path";

// spt imports
import type { DependencyContainer } from "tsyringe";
import type { IPostDBLoadMod } from "@spt/models/external/IPostDBLoadMod";
import type { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";

// WTT imports
import { WTTInstanceManager } from "./WTTInstanceManager";
import { CustomItemService } from "./CustomItemService";

// custom
import { ModGlobals } from "./Globals";

class PeinCarryHandleMod
implements IPreSptLoadMod, IPostDBLoadMod
{
    private Instance: WTTInstanceManager = new WTTInstanceManager();
    private version: string;
    private globals = ModGlobals;

    private customItemService: CustomItemService = new CustomItemService();

    debug = false;

    // Anything that needs done on preSptLoad, place here.
    public preSptLoad(container: DependencyContainer): void 
    {
    // Initialize the instance manager DO NOTHING ELSE BEFORE THIS
        this.Instance.preSptLoad(container, this.globals.modName);
        this.Instance.debug = this.debug;
        // EVERYTHING AFTER HERE MUST USE THE INSTANCE
        
        this.getVersionFromJson();
        this.displayCreditBanner();

        this.customItemService.preSptLoad(this.Instance);
    }

    // Anything that needs done on postDBLoad, place here.
    public async postDBLoadAsync(container: DependencyContainer): Promise<void> 
    {
    // Initialize the instance manager DO NOTHING ELSE BEFORE THIS
        this.Instance.postDBLoad(container);
        // EVERYTHING AFTER HERE MUST USE THE INSTANCE

        this.customItemService.postDBLoad();
    }

    private getVersionFromJson(): void 
    {
        const packageJsonPath = path.join(__dirname, "../package.json");

        fs.readFile(packageJsonPath, "utf-8", (err, data) => 
        {
            if (err) 
            {
                console.error("Error reading file:", err);
                return;
            }

            const jsonData = JSON.parse(data);
            this.version = jsonData.version;
        });
    }

    public colorLog(message: string, color: string) {
        const colorCodes = {
            red: "\x1b[31m",
            green: "\x1b[32m",
            yellow: "\x1b[33m",
            blue: "\x1b[34m",
            magenta: "\x1b[35m",
            cyan: "\x1b[36m",
            white: "\x1b[37m",
            gray: "\x1b[90m",
            brightRed: "\x1b[91m",
            brightGreen: "\x1b[92m",
            brightYellow: "\x1b[93m",
            brightBlue: "\x1b[94m",
            brightMagenta: "\x1b[95m",
            brightCyan: "\x1b[96m",
            brightWhite: "\x1b[97m"
        };
      
        const resetCode = "\x1b[0m";
        const colorCode = colorCodes[color as keyof typeof colorCodes] || "\x1b[37m"; // Default to white if color is invalid.
        console.log(`${colorCode}${message}${resetCode}`); // Log the colored message here
    }

    private displayCreditBanner(): void 
    {
        this.colorLog(`[${this.globals.modName}] Developer: pein | Framework: ${this.globals.frameworkAuthor} | ${this.globals.flavorText}`, "green");
    }
}

module.exports = { mod: new PeinCarryHandleMod() };
