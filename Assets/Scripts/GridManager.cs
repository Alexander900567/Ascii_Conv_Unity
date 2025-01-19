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
    public RectTransform grid_space_outline;
    public int col_count;
    public int row_count;
    private float col_size;
    private float row_size;
    private List<GameObject> grid_text_rows = new List<GameObject>();
    private List<List<char>> grid_array = new List<List<char>>();
    private List<(int, int, char)> preview_buffer = new List<(int, int, char)>(); //row, col, input

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col_size = (float) (Screen.width - ui_manager.ui_panel_transform.rect.width) / (float) col_count;
        row_size =  (float) Screen.height / (float) row_count;

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
                    ui_manager.ui_panel_transform.rect.width, 
                    row * row_size, 
                    0
                ), 
                transform.rotation
            ));
            grid_text_rows[0].transform.SetParent(canvas_transform);
            grid_text_rows[0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - ui_manager.ui_panel_transform.rect.width, row_size);
            grid_text_rows[0].name = "GridRow" + row.ToString();
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


        grid_space_outline.sizeDelta = new Vector2(col_size, row_size);
    }

    // Update is called once per frame
    void Update()
    {
        RenderGrid();
    }
    
    void RenderGrid(){
        String row_string = "";
        float col_position;
        float col_offset = col_size * (float) 0.33;
        float full_font_size = example_grid_row.GetComponent<TextMeshProUGUI>().fontSize;

        List<List<char>> render_array = new List<List<char>>(grid_array);
        foreach ((int, int, char) item in preview_buffer){
            render_array[item.Item1][item.Item2] = item.Item3;
        }

        for (int row = 0; row < row_count; row++){
            grid_text_rows[row].GetComponent<TextMeshProUGUI>().fontSize = full_font_size;
            for (int col = 0; col < col_count; col++){
                col_position = (col_size * col) + col_offset;
                row_string += "<pos=" + col_position.ToString("0.00") + "px>" + render_array[row][col];
            }
            grid_text_rows[row].GetComponent<TextMeshProUGUI>().text = row_string;
            row_string = "";
        } 

        Vector3 mouse_pos = Input.mousePosition;
        (int row, int col) grid_pos = get_grid_pos(mouse_pos, invert_row: false);
        grid_space_outline.anchoredPosition = new Vector2(col_size * grid_pos.col + ui_manager.ui_panel_transform.rect.width, row_size * grid_pos.row);


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

    public (int row, int col) get_grid_pos(Vector3 mouse_pos, bool invert_row=true){
        int col = (int) ((mouse_pos[0] - ui_manager.ui_panel_transform.rect.width) / col_size);
        int row = (int) (mouse_pos[1] / row_size);

        if (col < 0) { col = 0; }
        else if (col >= col_count) { col = col_count - 1; }
        if (row < 0) { row = 0; }
        else if (row >= row_count) { row = row_count - 1; }

        //col = col_count - 1 - col;
        if (invert_row){
            row = row_count - 1 - row;
        }
        return (row: row, col: col);
    }

}

/*
Component[] components = grid_text_cols[col].GetComponents(typeof(Component));
foreach(Component component in components) {
    Debug.Log(component.ToString());
}
*/