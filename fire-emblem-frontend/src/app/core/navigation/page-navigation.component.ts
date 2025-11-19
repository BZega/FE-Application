import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { ThemeService } from '../services/theme.service';

@Component({
  selector: 'app-page-navigation',
  imports: [CommonModule, RouterModule],
  templateUrl: './page-navigation.component.html',
  styleUrl: './page-navigation.component.scss'
})
export class PageNavigationComponent {
  isMenuOpen = false;
  isDarkMode$: Observable<boolean>;

  constructor(private themeService: ThemeService) {
    this.isDarkMode$ = this.themeService.darkMode$;
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}
