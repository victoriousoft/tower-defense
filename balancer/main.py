import os
import yaml
import json
from dataclasses import dataclass
from unidecode import unidecode
import shutil
import string
import pandas as pd

@dataclass
class Config:
    enemies: list[str]
    towers: list[str]

@dataclass
class EnemyData:
    name: str

    maxHealth: int
    damage: int
    cashDrop: int
    speed: int
    physicalResistance: int
    magicalResistance: int
    attackCooldown: int
    visRange: int
    playerLives: int
    isGround: bool
    isFlying: bool

    def generate_csv(self) -> str:
        return f"""\
{self.name}
{self.maxHealth}
{self.damage}
{self.cashDrop}
{self.speed}
{self.physicalResistance}
{self.magicalResistance}
{self.attackCooldown}
{self.visRange}
{self.playerLives}
{self.isGround}
{self.isFlying}"""


@dataclass
class TowerData:
    @dataclass
    class TowerLevel:
        price: int
        range: int
        damage: int
        cooldown: int

    @dataclass
    class Evolution:
        price: int
        name: str
        damage: int
        range: int

    name: str

    targetsFlying: bool
    targetsGround: bool
    physicalDamage: bool
    magicalDamage: bool

    levels: list[TowerLevel]
    evolutions: list[Evolution]

    def generate_csv(self) -> str:
        out = "name,"

        for i in range(len(self.levels)):
            out += f"{self.name} {i+1},"
        for i in range(len(self.evolutions)):
            out += f"{self.evolutions[i].name},"

        for key in self.levels[0].__dict__.keys():
            out += f"\n{key},"
            for level in self.levels:
                out += f"{level.__dict__[key]},"
            for evolution in self.evolutions:
                out += f"{evolution.__dict__.get(key, '')},"

        out += "\nPhysical dmg,"
        for i in range(len(self.levels) + len(self.evolutions)):
            out += f"{self.physicalDamage},"
        
        out += "\nMagical dmg,"
        for i in range(len(self.levels) + len(self.evolutions)):
            out += f"{self.magicalDamage},"

        out += "\nTargets flying,"
        for i in range(len(self.levels) + len(self.evolutions)):
            out += f"{self.targetsFlying},"
        
        out += "\nTargets ground,"
        for i in range(len(self.levels) + len(self.evolutions)):
            out += f"{self.targetsGround},"

        return out


def load_file_config(path: str) -> Config:
    with open(path, "r") as file:
        data = json.load(file)

    return Config(
        enemies=data.get("enemies", []),
        towers=data.get("towers", [])
    )


def load_yaml_config(path: str) -> dict:
    with open(path, "r") as file:
        lines = file.readlines()[3:]

    data = yaml.safe_load("".join(lines))["MonoBehaviour"]
    return {k: v for k, v in data.items() if not k.startswith("m_")}

def parse_enemy_file(path: str) -> EnemyData:
    data = load_yaml_config(path)

    stats = data.get("stats", {})
    info = data.get("info", {})

    enemyData = EnemyData(
        name=info.get("name", ""),
        maxHealth=stats.get("maxHealth", 0),
        damage=stats.get("damage", 0),
        cashDrop=stats.get("cashDrop", 0),
        speed=stats.get("speed", 0),
        physicalResistance=stats.get("physicalResistance", 0),
        magicalResistance=stats.get("magicalResistance", 0),
        attackCooldown=stats.get("attackCooldown", 0),
        visRange=stats.get("visRange", 0),
        playerLives=stats.get("playerLives", 0),
        isGround=data.get("EnemyType", 0) == 0,
        isFlying=data.get("EnemyType", 0) == 1
    )

    return enemyData

def parse_tower_file(path: str):
    data = load_yaml_config(path)

    targetsFlying = False
    targetsGround = False

    magicDamage = False
    physicalDamage = False

    name = data.get("towerName", "")

    # Jestli někdo přejmenoval věž, tak se tohle dojebe, z unity je to nedekódovatelný
    match name:
        case "Archers":
            targetsFlying = True
            targetsGround = True
            physicalDamage = True
        
        case "Bomber":
            targetsGround = True
            physicalDamage = True
        
        case "Maigc":
            targetsFlying = True
            targetsGround = True
            magicDamage = True
        
    towerData = TowerData(
        name=data.get("towerName", ""),
        targetsFlying=targetsFlying,
        targetsGround=targetsGround,
        physicalDamage=physicalDamage,
        magicalDamage=magicDamage,
        levels=[],
        evolutions=[]
    )

    for level in data.get("levels", []):
        towerData.levels.append(TowerData.TowerLevel(
            price=level.get("price", 0),
            range=level.get("range", 0),
            damage=level.get("damage", 0),
            cooldown=level.get("cooldown", 0)
        ))
    
    for evolution in data.get("evolutions", []):
        towerData.evolutions.append(TowerData.Evolution(
            price=evolution.get("price", 0),
            name=evolution.get("name", ""),
            damage=evolution.get("damage", 0),
            range=evolution.get("range", 0)
        ))

    return towerData


def prepare_out_directories():
    os.makedirs("./out", exist_ok=True)
    if os.path.exists("./out"):
        shutil.rmtree("./out")

    os.makedirs("./out/enemies", exist_ok=True)
    os.makedirs("./out/towers", exist_ok=True)

def generate_safe_filename(name: str) -> str:
    valid_chars = f"_{string.ascii_lowercase}{string.digits}"
    return ''.join(c for c in unidecode(name.lower()) if c in valid_chars)

if __name__ == "__main__":
    config = load_file_config("./files.json")

    enemies: list[EnemyData] = []    
    for path in config.enemies:
        enemies.append(parse_enemy_file(path))

    towers: list[TowerData] = []
    for path in config.towers:
        towers.append(parse_tower_file(path))
    
    prepare_out_directories()

    for enemy in enemies:
        with open(f"./out/enemies/{generate_safe_filename(enemy.name)}.csv", "w", encoding='utf-8') as file:
            file.write(enemy.generate_csv())
    
    for tower in towers:
        with open(f"./out/towers/{generate_safe_filename(tower.name)}.csv", "w", encoding='utf-8') as file:
            file.write(tower.generate_csv())

    combined_enemies = pd.DataFrame()
    for file in os.listdir("./out/enemies"):
        df = pd.read_csv(f"./out/enemies/{file}", header=None).T
        df.columns = list(EnemyData.__annotations__.keys())
        combined_enemies = pd.concat([combined_enemies, df], axis=0)

    
    combined_towers = pd.DataFrame()
    for file in os.listdir("./out/towers"):
        df = pd.read_csv(f"./out/towers/{file}", header=None).T
        df.columns = df.iloc[0]
        df = df[1:]
        combined_towers = pd.concat([combined_towers, df], axis=0).dropna(how='all')

    combined_towers_csv = combined_towers.to_csv(index=False).replace("\r", "").split("\n")
    combined_enemies_csv = combined_enemies.to_csv(index=False).replace("\r", "").split("\n")

    combined_csv = ""
    max_lines = max(len(combined_towers_csv), len(combined_enemies_csv)) - 1
    tower_indent_length = len(combined_towers_csv[0].split(","))
    enemy_indent = "," * tower_indent_length

    for i in range(max_lines):
        if i < len(combined_towers_csv) - 1:
            tower_line = combined_towers_csv[i]
        else:
            tower_line = "," * (tower_indent_length - 1)
        
        if i < len(combined_enemies_csv):
            enemy_line = combined_enemies_csv[i]
            if i > len(combined_towers_csv):
                combined_csv += f"{enemy_indent},{enemy_line}\n"
            else:
                combined_csv += f"{tower_line},,{enemy_line}\n"
        else:
            combined_csv += f"{tower_line}\n"

    with open("./out/combined.csv", "w", encoding='utf-8') as file:
        file.write(combined_csv)

    
    
