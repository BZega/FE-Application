import { Component, Input } from '@angular/core';
import { Observable, map } from 'rxjs';
import { Character } from '../../core/models/Character';
import { CharacterService } from '../../core/services/character.service';

@Component({
  selector: 'character-summary',
  imports: [],
  templateUrl: './character-summary.component.html',
  styleUrl: './character-summary.component.scss'
})
export class CharacterSummaryComponent {
  @Input() character: Character;
  @Input() index: number;
  selectedCharacterNumber$: Observable<number | null>;
  
  
  constructor(
    private characterService: CharacterService
  ) { }
  ngOnInit(): void {
    this.selectedCharacterNumber$ = this.characterService.selectedCharacter$.pipe(map(character =>  character?.id ?? null ));
  }

  isSelected(): Observable<boolean> {
    return this.selectedCharacterNumber$.pipe(map(x => x === this.character.id));
  }

  onSelect(event: Event) {
    return null;
  }
}
