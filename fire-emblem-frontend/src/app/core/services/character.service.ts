import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Character } from '../models/Character';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {

  public selectedCharacter = new BehaviorSubject<Character>(null);
  public selectedCharacter$ = this.selectedCharacter.asObservable();

  public characterList = new BehaviorSubject<Character[]>(null);
  public characterList$ = this.characterList.asObservable();

  constructor(
      private http: HttpClient
  ) { }

  public getAllCharacters(): Observable<Character[]> {
      const url = 'https://localhost:7145/Characters/get-all-characters';
      return this.http.get<Character[]>(url);
  }
}
