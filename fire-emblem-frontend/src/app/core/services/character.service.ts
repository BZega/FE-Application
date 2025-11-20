import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Character } from '../models/Character';
import { UnitClass } from '../models/UnitClass';
import { PersonalAbility } from '../models/PersonalAbility';
import { Ability } from '../models/Ability';
import { Equipment } from '../models/Equipment';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {

  private http = inject(HttpClient);

  public selectedCharacter = new BehaviorSubject<Character>(null);
  public selectedCharacter$ = this.selectedCharacter.asObservable();

  public characterList = new BehaviorSubject<Character[]>(null);
  public characterList$ = this.characterList.asObservable();

  public getAllCharacters(): Observable<Character[]> {
      const url = 'https://localhost:7145/Characters/get-all-characters';
      return this.http.get<Character[]>(url);
  }
  public getCharacterById(id: number): Observable<Character> {
    const url = `https://localhost:7145/Characters/get-character/${id}`;
    return this.http.get<Character>(url);
  }
  public updateEquipedAbilities(characterId: number, abilityOid: string, equipType: string): Observable<any> {
    const url = `https://localhost:7145/Characters/update-equipped-abilities/${abilityOid}?characterId=${characterId}&updateType=${equipType}`;
    return this.http.post(url, {});
  }
  public createCharacter(name: string, characterPayload: any): Observable<Character> {
    const url = `https://localhost:7145/Characters/add-new-character/${name}`;
    return this.http.post<Character>(url, { ...characterPayload });
  }
  public getClass(name: string): Observable<UnitClass> {
    const url = `https://localhost:7145/UnitClasses/get-class-by-name/${name}`;
    return this.http.get<UnitClass>(url);
  }
  public getAbility(name: string): Observable<Ability> {
    const url = `https://localhost:7145/Abilities/get-ability-by-name/${name}`;
    return this.http.get<Ability>(url);
  }
  public getPersonalAbility(name: string): Observable<PersonalAbility> {
    const url = `https://localhost:7145/PersonalAbilities/get-personal-ability-by-name/${name}`;
    return this.http.get<PersonalAbility>(url);
  }
  public getAllEquipment(): Observable<Equipment[]> {
    const url = `https://localhost:7145/Equipment/get-all-equipment`;
    return this.http.get<Equipment[]>(url);
  }
  public getEquipmentByName(name: string): Observable<Equipment> {
    const url = `https://localhost:7145/Equipment/get-equipment-by-name/${name}`;
    return this.http.get<Equipment>(url);
  }
}
