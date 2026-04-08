#!/usr/bin/env python3
"""
Created by GrooveypenguinX (and presumably ai magic)

Locale Tool
Scrapes locales from multiple config setups and injects them into a unified en.json.
Normalizes locale keys like "id name"/"id shortName"/"id description" so that:
- Suffix is canonical: Name, ShortName, Description, Location, Nickname, FullName, FirstName
- Keys differing only by suffix case are merged into one.
"""

import json
from pathlib import Path
from typing import Dict, Any, Optional
from dataclasses import dataclass
import tkinter as tk
from tkinter import filedialog, messagebox
from tkinter import ttk
import threading


@dataclass
class LocaleConfig:
    """Configuration for a specific locale source"""
    name: str
    folder: str
    processor: callable


class LocaleScraper:
    def __init__(self):
        self.root_path: Optional[Path] = None
        self.locales: Dict[str, Any] = {}
        self.stats = {
            'items': 0,
            'heads': 0,
            'voices': 0,
            'clothing': 0,
            'achievements': 0,
            'quests': 0,
            'updated_keys': 0,
            'new_keys': 0,
            'normalized_keys': 0,
        }

        # Canonical suffix mapping for top-level keys like "id Name"
        self.suffix_map = {
            "name": "Name",
            "shortname": "ShortName",
            "description": "Description",
            "location": "Location",
            "nickname": "Nickname",
            "fullname": "FullName",
            "firstname": "FirstName",
            "startedmessagetext": "StartedMessageText",
            "successmessagetext": "SuccessMessageText",
            "failmessagetext": "FailMessageText",
            "acceptplayermessage": "AcceptPlayerMessage",
            "declineplayermessage": "DeclinePlayerMessage",
            "completeplayermessage": "CompletePlayerMessage",
            "note": "Note",
        }

        # paths relative to Resources/
        self.resources_paths = {
            "CustomItems": "db/CustomItems",
            "CustomHeads": "db/CustomHeads",
            "CustomVoices": "db/CustomVoices",
            "CustomClothing": "db/CustomClothing",
            "CustomAchievements": "db/CustomAchievements",
            "CustomQuests": "db/CustomQuests",
            "OutputPath": "db/CustomLocales"
        }

    def get_resources_folder(self) -> Optional[Path]:
        cwd = Path.cwd()
        resources = cwd.joinpath("Resources")

        if resources:
            return Path(resources)
        return None

    def get_paths_map(self) -> Optional[Dict[str, str]]:
        cwd = Path.cwd()
        paths = json.load(open(cwd.joinpath("resources_dirs.json"), "r", encoding="utf-8"))

        if paths:
            self.resources_paths = paths
            return self.resources_paths
        
        return None

    def load_existing_locales(self, outPath : str) -> bool:
        """Load and normalize existing en.json if it exists"""
        locale_path = self.root_path / outPath / "en.json"
        if locale_path.exists():
            try:
                with open(locale_path, 'r', encoding='utf-8') as f:
                    raw = json.load(f)
                # Normalize all existing keys so old lowercase entries are fixed too
                for key, value in raw.items():
                    norm_key = self._normalize_locale_key(key)
                    # Later entries win if duplicates exist
                    if norm_key in self.locales:
                        self.stats['updated_keys'] += 1
                    else:
                        self.stats['new_keys'] += 1
                    self.locales[norm_key] = value
                print(f"✓ Loaded existing en.json with {len(raw)} raw keys → {len(self.locales)} normalized keys")
                return True
            except Exception as e:
                print(f"✗ Error loading existing en.json: {e}")
                return False
        else:
            print(f"✓ No existing en.json found, starting fresh")
            return True

    # ---------- Key normalization helpers ----------

    def _normalize_locale_key(self, raw_key: str) -> str:
        """
        Normalize a top-level locale key of the form:
          "id name", "id Name", "id shortName", "id SHORTNAME", etc.
        into:
          "id Name", "id ShortName", etc.
        Keys without a space or with unknown suffix are left as-is.
        """
        parts = raw_key.split(" ", 1)
        if len(parts) != 2:
            return raw_key

        base_id, suffix = parts
        canon = self.suffix_map.get(suffix.lower())
        if canon:
            norm_key = f"{base_id} {canon}"
            if norm_key != raw_key:
                self.stats['normalized_keys'] += 1
                print(f"  Normalized key '{raw_key}' → '{norm_key}'")
            return norm_key

        return raw_key

    # ---------- Processors ----------

    def process_custom_items(self, resPath : str) -> int:
        """Process CustomItems folder"""
        items_path = self.root_path / resPath
        if not items_path.exists():
            print(f"⊘ CustomItems folder not found")
            return 0

        count = 0
        for json_file in items_path.rglob("*.json"):
            try:
                with open(json_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)

                for item_id, item_config in data.items():
                    if isinstance(item_config, dict) and "locales" in item_config:
                        locales_en = item_config["locales"].get("en", {})
                        if isinstance(locales_en, dict):
                            for key, value in locales_en.items():
                                raw_key = f"{item_id} {key}"
                                self._add_or_update_locale(raw_key, value)
                                count += 1
            except Exception as e:
                print(f"✗ Error processing {json_file}: {e}")

        self.stats['items'] = count
        print(f"✓ Processed CustomItems: {count} locale keys")
        return count

    def process_custom_heads(self, resPath : str) -> int:
        """Process CustomHeads folder"""
        heads_path = self.root_path / resPath
        if not heads_path.exists():
            print(f"⊘ CustomHeads folder not found")
            return 0

        count = 0
        for json_file in heads_path.rglob("*.json"):
            try:
                with open(json_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)

                for head_id, head_config in data.items():
                    if isinstance(head_config, dict) and "locales" in head_config:
                        value = head_config["locales"].get("en")
                        if value:
                            # Head keys are usually just IDs, no suffix
                            self._add_or_update_locale(head_id, value)
                            count += 1
            except Exception as e:
                print(f"✗ Error processing {json_file}: {e}")

        self.stats['heads'] = count
        print(f"✓ Processed CustomHeads: {count} locale keys")
        return count

    def process_custom_voices(self, resPath : str) -> int:
        """Process CustomVoices folder"""
        voices_path = self.root_path / resPath
        if not voices_path.exists():
            print(f"⊘ CustomVoices folder not found")
            return 0

        count = 0
        for json_file in voices_path.rglob("*.json"):
            try:
                with open(json_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)

                for voice_id, voice_config in data.items():
                    if isinstance(voice_config, dict) and "locales" in voice_config:
                        value = voice_config["locales"].get("en")
                        if value:
                            self._add_or_update_locale(voice_id, value)
                            count += 1
            except Exception as e:
                print(f"✗ Error processing {json_file}: {e}")

        self.stats['voices'] = count
        print(f"✓ Processed CustomVoices: {count} locale keys")
        return count

    def process_custom_clothing(self, resPath : str) -> int:
        """Process CustomClothing folder"""
        clothing_path = self.root_path / resPath
        if not clothing_path.exists():
            print(f"⊘ CustomClothing folder not found")
            return 0

        count = 0
        for json_file in clothing_path.rglob("*.json"):
            try:
                with open(json_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)

                for clothing_config in (data if isinstance(data, list) else [data]):
                    if isinstance(clothing_config, dict):
                        suite_id = clothing_config.get("suiteId")
                        locales_en = clothing_config.get("locales", {}).get("en", {})

                        if suite_id and isinstance(locales_en, dict):
                            for key, value in locales_en.items():
                                raw_key = f"{suite_id} {key}"
                                self._add_or_update_locale(raw_key, value)
                                count += 1
            except Exception as e:
                print(f"✗ Error processing {json_file}: {e}")

        self.stats['clothing'] = count
        print(f"✓ Processed CustomClothing: {count} locale keys")
        return count

    def process_custom_achievements(self, resPath : str) -> int:
        """Process CustomAchievements/Locales/en.json"""
        achievements_path = self.root_path / resPath
        if not achievements_path.exists():
            print(f"⊘ CustomAchievements locales not found")
            return 0

        count = 0
        try:
            with open(achievements_path, 'r', encoding='utf-8') as f:
                data = json.load(f)
                for key, value in data.items():
                    self._add_or_update_locale(key, value)
                    count += 1
        except Exception as e:
            print(f"✗ Error processing CustomAchievements: {e}")

        self.stats['achievements'] = count
        print(f"✓ Processed CustomAchievements: {count} locale keys")
        return count

    def process_custom_quests(self, resPath : str) -> int:
        """Process CustomQuests locales recursively"""
        quests_path = self.root_path / resPath
        if not quests_path.exists():
            print(f"⊘ CustomQuests folder not found")
            return 0

        count = 0
        for locale_file in quests_path.rglob("Locales/en.json"):
            try:
                with open(locale_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                    for key, value in data.items():
                        self._add_or_update_locale(key, value)
                        count += 1
            except Exception as e:
                print(f"✗ Error processing {locale_file}: {e}")

        self.stats['quests'] = count
        print(f"✓ Processed CustomQuests: {count} locale keys")
        return count

    # ---------- Core locale merge ----------

    def _add_or_update_locale(self, raw_key: str, value: Any) -> None:
        """Add new or update existing locale key using normalized key."""
        norm_key = self._normalize_locale_key(raw_key)

        if norm_key in self.locales:
            self.stats['updated_keys'] += 1
        else:
            self.stats['new_keys'] += 1

        self.locales[norm_key] = value

    def save_locales(self, outPath : str) -> bool:
        """Save aggregated locales to en.json"""
        output_path = self.root_path / outPath
        output_path.mkdir(parents=True, exist_ok=True)

        locale_file = output_path / "en.json"
        try:
            with open(locale_file, 'w', encoding='utf-8') as f:
                json.dump(self.locales, f, ensure_ascii=False, indent=2)
            print(f"✓ Saved {len(self.locales)} total normalized locale keys to en.json")
            return True
        except Exception as e:
            print(f"✗ Error saving en.json: {e}")
            return False

    def run(self) -> bool:
        """Execute the aggregation pipeline"""
        print("=" * 60)
        print("Locale Scraper Tool (with key normalization)")
        print("=" * 60)

        # Select root folder
        self.root_path = self.get_resources_folder()
        if not self.root_path:
            print("✗ No resources folder found")
            return False
        
        self.get_paths_map()

        # Load existing locales (and normalize them)
        if not self.load_existing_locales(self.resources_paths["OutputPath"]):
            return False
        print()

        # Process all sources
        print("Processing locale sources...")
        self.process_custom_items(self.resources_paths["CustomItems"])
        self.process_custom_heads(self.resources_paths["CustomHeads"])
        self.process_custom_voices(self.resources_paths["CustomVoices"])
        self.process_custom_clothing(self.resources_paths["CustomClothing"])
        self.process_custom_achievements(self.resources_paths["CustomAchievements"])
        self.process_custom_quests(self.resources_paths["CustomQuests"])
        print()

        print(f"✓ Normalized {self.stats['normalized_keys']} top-level keys (Name, ShortName, etc.)")
        print()

        # Save results
        if not self.save_locales(self.resources_paths["OutputPath"]):
            return False
        print()

        # Print summary
        print("=" * 60)
        print("Summary")
        print("=" * 60)
        print(f"Items:           {self.stats['items']} keys")
        print(f"Heads:           {self.stats['heads']} keys")
        print(f"Voices:          {self.stats['voices']} keys")
        print(f"Clothing:        {self.stats['clothing']} keys")
        print(f"Achievements:    {self.stats['achievements']} keys")
        print(f"Quests:          {self.stats['quests']} keys")
        print(f"Normalized:      {self.stats['normalized_keys']} keys")
        print(f"─────────────────────────────")
        print(f"New keys:        {self.stats['new_keys']}")
        print(f"Updated keys:    {self.stats['updated_keys']}")
        print(f"Total keys:      {len(self.locales)}")
        print("=" * 60)

        return True


def main():
    scraper = LocaleScraper()
    success = scraper.run()

    root = tk.Tk()
    root.withdraw()
    if success:
        messagebox.showinfo("Success", f"Locale aggregation complete!\n\nTotal keys: {len(scraper.locales)}")
    else:
        messagebox.showerror("Error", "Locale aggregation failed. Check console for details.")
    root.destroy()


if __name__ == "__main__":
    main()
