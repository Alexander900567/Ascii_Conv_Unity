using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{

    public GameObject grid_text_row;
    public UIManager ui_manager;
    public RectTransform canvas_transform;
    public GameObject example_grid_row;
    public int col_count;
    public int row_count;
    private float col_size;
    private float row_size;
    private String background_color;
    private List<GameObject> grid_text_rows = new List<GameObject>();
    private List<List<char>> grid_array = new List<List<char>>();
    private List<(int, int, char)> preview_buffer = new List<(int, int, char)>(); //row, col, input

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col_size = (Screen.width - ui_manager.ui_panel_transform.rect.width) / col_count;
        row_size = Screen.height / row_count;
        background_color = "black";

        for (int row = 0; row < row_count; row++){
            grid_array.Add(new List<char>());
            for (int col = 0; col < col_count; col++){
                grid_array[row].Add(' ');
            }
        }

        for (int row = 0; row < row_count; row++){
            grid_text_rows.Insert(0, Instantiate(
                grid_text_row, 
                new Vector3(
                    ui_manager.ui_panel_transform.rect.width + ((Screen.width - ui_manager.ui_panel_transform.rect.width) / 2), 
                    row * row_size + (row_size / 2), 
                    0
                ), 
                transform.rotation
            ));
            grid_text_rows[0].transform.SetParent(canvas_transform);
            grid_text_rows[0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - ui_manager.ui_panel_transform.rect.width, row_size);
            /*
            Component[] components = grid_text_cols[col].GetComponents(typeof(Component));
            foreach(Component component in components) {
                Debug.Log(component.ToString());
            }
            */
        } 
        

        String auto_size_string = "";
        for(int col = 0; col < col_count; col++){
            auto_size_string += "N ";
        }
        example_grid_row.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - ui_manager.ui_panel_transform.rect.width, row_size);
        example_grid_row.GetComponent<TextMeshProUGUI>().text = auto_size_string;
    }

    // Update is called once per frame
    void Update()
    {
        RenderGrid();
    }
    
    void RenderGrid(){
        String row_string = "";
        float full_font_size = example_grid_row.GetComponent<TextMeshProUGUI>().fontSize;

        List<List<char>> render_array = new List<List<char>>(grid_array);
        foreach ((int, int, char) item in preview_buffer){
            render_array[item.Item1][item.Item2] = item.Item3;
        }

        for (int row = 0; row < row_count; row++){
            grid_text_rows[row].GetComponent<TextMeshProUGUI>().fontSize = full_font_size;
            for (int col = 0; col < col_count; col++){
                if (render_array[row][col] == ' '){
                    row_string += "<color=\""+ background_color +"\">.</color>";
                }
                else{
                    row_string += render_array[row][col];
                }
                row_string += " ";
            }
            grid_text_rows[row].GetComponent<TextMeshProUGUI>().text = row_string;
            row_string = "";
        } 
    }

    public void add_to_preview_buffer(int row, int col, String input){
        Debug.Assert(input.Length == 1, "input can only be one character in add_to_preview_buffer");

        if (row >= row_count || row < 0){ return; }
        else if (col >= col_count || col < 0){ return; }

        preview_buffer.Add((row, col, input[0]));
    }

    public void write_pbuffer_to_array(){
        foreach ((int, int, char) item in preview_buffer){
            grid_array[item.Item1][item.Item2] = item.Item3;
        }
        preview_buffer.Clear();
    }

    public (int row, int col) get_grid_pos(Vector3 mouse_pos){
        int col = (int) ((mouse_pos[0] - ui_manager.ui_panel_transform.rect.width) / col_size);
        int row = (int) (mouse_pos[1] / row_size);

        if (col < 0) { col = 0; }
        else if (col >= col_count) { col = col_count - 1; }
        if (row < 0) { row = 0; }
        else if (row >= row_count) { row = row_count - 1; }

        //col = col_count - 1 - col;
        row = row_count - 1 - row;

        return (row: row, col: col);
    }

}

/*
Component[] components = grid_text_cols[col].GetComponents(typeof(Component));
foreach(Component component in components) {
    Debug.Log(component.ToString());
}
*/