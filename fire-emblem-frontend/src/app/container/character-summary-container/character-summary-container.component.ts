import { Component } from '@angular/core';
import { Character } from '../../core/models/Character';
import { Observable, Subject } from 'rxjs';
import { CharacterService } from '../../core/services/character.service';
// import { CharacterSummaryComponent } from '../../components/character-summary/character-summary.component';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material/material.module';
import { CharacterSummaryComponent } from '../../components/character-summary/character-summary.component';

@Component({
  selector: 'app-character-summary-container',
  imports: [CommonModule, MaterialModule, CharacterSummaryComponent],
  templateUrl: './character-summary-container.component.html',
  styleUrl: './character-summary-container.component.scss'
})
export class CharacterSummaryContainerComponent {
  trackCharactersBy(index: number, character: { id: number }): number {
    return character.id;
  }
  selectedCharacter$ = null;
  selectedCharacter: Character = null;
  allCharacters: Character[] = [];
  characterList$: Observable<Character[]>;

  private ngUnsubscribe = new Subject();

  constructor(
      private characterService: CharacterService
  ) { }

  ngOnInit(): void {
    this.characterService.getAllCharacters().subscribe({
      next: (data) => {
        console.log("Received data:", data);
        this.allCharacters = data;
        this.characterService.characterList.next(this.allCharacters);
      },
      error: (error) => console.error("HTTP Error:", error),
      complete: () => console.log("Request complete"),
    })

    this.characterList$ = this.characterService.characterList$;
    console.log("Brandon - ", this.characterList$);
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next(null);
    this.ngUnsubscribe.complete();
  }
}
