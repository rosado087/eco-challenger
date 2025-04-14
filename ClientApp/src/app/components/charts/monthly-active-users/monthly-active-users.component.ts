import { Component, inject, input, OnInit } from '@angular/core';
import { heroArrowLeft, heroArrowRight } from '@ng-icons/heroicons/outline'
import { NgIcon, provideIcons } from '@ng-icons/core';
import type { EChartsCoreOption } from 'echarts/core';
import { NgxEchartsDirective } from 'ngx-echarts';
import { provideEchartsCore } from 'ngx-echarts'
import * as echarts from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers';
import { GridComponent, LegendComponent, TitleComponent, TooltipComponent } from 'echarts/components'
import { NetApiService } from '../../../services/net-api/net-api.service';
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service';
import { MonthlyActiveUsers } from '../../../models/stats-models';
import { LineChart } from 'echarts/charts';

echarts.use([
  LineChart,
  CanvasRenderer
])

@Component({
  selector: 'app-monthly-active-users',
  imports: [NgIcon, NgxEchartsDirective],
  templateUrl: './monthly-active-users.component.html',
  styleUrl: './monthly-active-users.component.css',
  providers: [
    provideEchartsCore({ echarts }),
    provideIcons({ heroArrowLeft, heroArrowRight }),
    PopupLoaderService
  ]
})
export class MonthlyActiveUsersComponent implements OnInit {
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  option: EChartsCoreOption | null = null
  height = input<string>('300px')

  year = new Date().getFullYear()
  month = new Date().getMonth() + 1 // For some reason, JS starts this at 0

  ngOnInit(): void {
    this.fetchChartData()
  }

  getMonthName(): string {
    switch(this.month) {
      case 1: return 'Janeiro'
      case 2: return 'Fevereiro'
      case 3: return 'Março'
      case 4: return 'Abril'
      case 5: return 'Maio'
      case 6: return 'Junho'
      case 7: return 'Julho'
      case 8: return 'Agosto'
      case 9: return 'Setembro'
      case 10: return 'Outubro'
      case 11: return 'Novembro'
      case 12: return 'Dezembro'
      default: return ''
    }
  }

  previousMonth() {
    if(this.month == 1) {
      this.month = 12
      this.year--
      return
    }

    this.month--
    this.fetchChartData()
  }

  nextMonth() {
    if(this.month == 12) {
      this.month = 1
      this.year++
      return
    }

    this.month++
    this.fetchChartData()
  }

  fetchChartData(): void {
    const params = { year: this.year.toString(), month: this.month.toString()}
    this.netApi
    .get<{logins: MonthlyActiveUsers[]}>('Statistics', 'monthly-active-users', params)
    .subscribe({
        next: (r) => this.loadChartData(r.logins),
        error: () => this.popupLoader.showPopup('Erro ao carregar top utilizadores ativos mensais.')
    })
  }

  loadChartData(data: MonthlyActiveUsers[]): void {
    this.option = {
      xAxis: {
        type: 'category',
        data: data.map(d => d.date)
      },
      legend: {
        data: ['Quantidade de Logins']
      },
      tooltip: {
        trigger: 'axis'
      },
      yAxis: {
        type: 'value'
      },
      series: [
        {
          name: 'Quantidade de Logins',
          data: data.map(d => d.count),
          type: 'line',
          smooth: true
        }
      ]
    }
  }
}
