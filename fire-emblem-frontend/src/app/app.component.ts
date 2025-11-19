import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PageNavigationComponent } from './core/navigation/page-navigation.component';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    PageNavigationComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'fe-frontend';

  constructor(private themeService: ThemeService) {
    // Theme service will initialize automatically
  }
}
